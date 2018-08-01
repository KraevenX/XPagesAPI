using System;
using System.Collections;

/// <summary>
/// Connector Object - Used to connect to a domino database - by interfacing with a custom XPages database and Rest Service
/// </summary>
/// <example>The following is an example of initializing a
/// <c>Connector</c> type:
///   <code>
///     // Create the connector
///     Connector connector = new Connector(UserName, Password, serverURL);
///     // Initialize the connector
///     connector.Initialize();
///     //must always be called, possibility to add pass,iv,salt by default this is already set
///     if (connector.isInitialized) {
///         if (connector.Connect()) {  // establish a connection to the domino server
///
///             // From the connector you can now create a session object
///             // The session object gives you the possibility to get a database object
///             // From that database you can get the actual document objects
///             // From those document objects you can then get the field objects (key/value)
///         }
///     }
///   </code>
/// </example>
public class Connector {

    #region Variables
    private static bool _hasError = false;
    internal Requestor Request;

    /// <summary>
    /// A boolean to trigger the code to throw an exception when an error has been encountered.
    /// <para>If set to false, the errors are just stored in the static list ReturnMessages and it will be indicated by isError boolean variable</para>
    /// </summary>
    public static bool ThrowException = false;

    /// <summary>
    /// Variable containing the supplied UserName (read-only)
    /// </summary>
    public readonly string UserName;

    /// <summary>
    /// Variable containing the supplied Password (read-only)
    /// </summary>
    public readonly string Password;

    /// <summary>
    /// Variable containing the supplied Server URL (read-only)
    /// </summary>
    public readonly string ServerURL;

    #endregion Variables

    #region Constructor

    /// <summary>
    /// Constructor method, provide the users credentials to logon to a domino server
    /// </summary>
    /// <param name="User"></param>
    /// <param name="Pass"></param>
    /// <param name="server"></param>
    public Connector(string User, string Pass, string server) {
        ResetReturn();
        AppDomain.CurrentDomain.UnhandledException += new System.UnhandledExceptionEventHandler(Common.ExceptionHandler);

        if (User != null && Pass != null && server != null) {
            this.UserName = User;
            this.Password = Pass;
            this.ServerURL = server;
            // validate server -> contains http:// ?
            if (server.ToLower().Contains("http://") || server.ToLower().Contains("https://")) {
                IsInitialized = true;
            } else {
                // not a valid URL
                ReturnMessages.Add("Invalid Server URL Provided : " + server);
                HasError = true;
            }
        } else {
            HasError = false;
        }
    }

    #endregion Constructor

    #region Static Properties

    /// <summary>
    /// This boolean will indicate if an error has occurred in the library and based on the ThrowException variable it will thow an execption when hasError is true
    /// </summary>
    public static bool HasError {
        get {
            return _hasError;
        }
        protected internal set {
            _hasError = value;
            if (_hasError) {
                if (ThrowException) {
                    throw new Exception(Common.GetListAsString(Connector.ReturnMessages, Environment.NewLine));
                }
            }
        }
    }

    /// <summary>
    /// A list of messages generated in the library
    /// <para>This list is used as a return mechanism of actions performed in this library</para>
    /// </summary>
    public static ArrayList ReturnMessages { get; set; } = new ArrayList();

    #endregion Static Properties

    #region Properties

    /// <summary>
    /// Indicates if a connection to the domino server has been established
    /// </summary>
    public bool IsConnected { get; private set; } = false;

    /// <summary>
    /// Indicates if the connector has been initialized
    /// <para>When not initialized no other methods can be executed</para>
    /// </summary>
    public bool IsInitialized { get; private set; } = false;

    /// <summary>
    /// A custom identification header, used when sending request to the domino database
    /// </summary>
    public string XPIdentity { get; set; } = "";

    /// <summary>
    /// Encryption IV Key
    /// </summary>
    protected internal string EncryptionIV { get; set; } = "";

    /// <summary>
    /// Encryption Password
    /// </summary>
    protected internal string EncryptionPASSWORD { get; set; } = "";

    /// <summary>
    /// Encryption Salt Key
    /// </summary>
    protected internal string EncryptionSALT { get; set; } = "";

    /// <summary>
    /// A SessionObject is used to establish a connection to the XPages database that hosts the Rest Service
    /// <para>In order to get the SessionObject, the user needs to have access to the XPages database</para>
    /// </summary>
    public SessionObject SessionObject { get; private set; } = null;

    #endregion Properties

    #region Public Methods

    /// <summary>
    /// Initializes the Connector and provides the possibility to use custom encryption keys and Identification header
    /// </summary>
    /// <param name="EncryptionPassword"></param>
    /// <param name="EncryptionIV"></param>
    /// <param name="EncryptionSalt"></param>
    /// <param name="Identify"></param>
    /// <returns>bool</returns>
    public bool Initialize(string EncryptionPassword = null, string EncryptionIV = null, string EncryptionSalt = null, string Identify = null) {
        ResetReturn();
        //no need to supply unless changes in XPages database - this will provide a way to enter new paswords
        if (EncryptionPassword != null) {
            this.EncryptionPASSWORD = EncryptionPassword;
        }
        if (EncryptionIV != null) {
            this.EncryptionIV = EncryptionIV;
        }
        if (EncryptionSalt != null) {
            this.EncryptionSALT = EncryptionSalt;
        }
        if (Identify != null) {
            XPIdentity = Identify;
        }
        if (!_hasError) {
            IsInitialized = true;
        } else {
            IsInitialized = false;
        }
        return IsInitialized;
    }

    /// <summary>
    /// When the connector has been initialized, this method will establish a connection to the domino server using the provided credentials
    /// <para>Sets isConnected property</para>
    /// </summary>
    /// <returns>bool</returns>
    public bool Connect() {
        // only try to connect if initialized - all info needed is available
        if (IsInitialized) {
            ResetReturn();
            // reset the error vars - will be populated by Requestor
            Request = new Requestor(this);
            // creates connection to domino and tries to login with supplied credentials
            if (Request.Initialize()) {
                IsConnected = true;
                return true;
            } else {
                //unable to connect - hasError set to true & returnmessages added
                IsConnected = false;
                return false;
            }
        } else {
            //hasError is set to true (New()) and returnmessage contains reason
            IsConnected = false;
            return false;
        }
    }

    /// <summary>
    /// This method will try to create a SessionObject, by connecting to the XPages database Rest Service
    /// <para>In order to get the SessionObject, the user needs to have access to the XPages database</para>
    /// </summary>
    /// <param name="DominoWebServiceURL"></param>
    /// <returns>SessionObject</returns>
    public SessionObject GetSession(string DominoWebServiceURL) {
        ResetReturn();
        // reset the error vars - will be populated by SessionObject
        if (IsConnected) {
            //we have a valid Requestor
            if (Request != null && Request.isInitialized) {
                SessionObject sObj = new SessionObject(this, DominoWebServiceURL);
                if (sObj.Initialize()) {
                    SessionObject = sObj;
                    return sObj;
                    //not written in initialize - just returning false
                } else {
                    ReturnMessages.Add("Connector.GetSession unable to complete, unable to initialize the session object");
                    HasError = true;
                    //throws exception
                    return null;
                }
            } else {
                ReturnMessages.Add("Connector.GetSession unable to complete, the request object is invalid or not initialized");
                HasError = true;
                //throws exception
                return null;
            }
        } else {
            ReturnMessages.Add("Connector.GetSession unable to complete, Connector is not connected to domino");
            HasError = true;  //throws exception
            return null;
        }
    }

    #endregion Public Methods

    #region Internal Methods

    /// <summary>
    /// Internal method to reset the return mechanism
    /// </summary>
    static internal void ResetReturn() {
        HasError = false;
        ReturnMessages = new ArrayList();
    }

    #endregion Internal Methods
}