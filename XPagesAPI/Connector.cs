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

    private bool _isInitialized = false;
    private bool _isConnected = false;
    private static bool _hasError = false;
    private SessionObject _SessionObject = null;

    private static ArrayList _ReturnMessages = new ArrayList();

    internal Requestor Request;

    private string _EncryptionIV = ""; // "GeNar@tEdIv_K3y!";    //"IV_VALUE_16_BYTE"                                                                   
    private string _EncryptionPASSWORD = ""; //  "JPITeam@XPages!|";  //"PASSWORD_VALUE"
    private string _EncryptionSALT = ""; //  "S@1tS@lt_Valu3@JPI_XP@ges";     //"SALT_VALUE"                                                                               
    private string _XPIdentity = ""; // "JPI$XP@ges!C0nn3nt0r|Id3nTity@Request";  // used to identify the request is coming from an approved source

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

    #endregion

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
                _isInitialized = true;
            } else {
                // not a valid URL
                ReturnMessages.Add("Invalid Server URL Provided : " + server);
                hasError = true;
            }
        } else {
            hasError = false;
        }
    }

    #endregion

    #region Static Properties
    
    /// <summary>
    /// This boolean will indicate if an error has occurred in the library and based on the ThrowException variable it will thow an execption when hasError is true
    /// </summary>
    public static bool hasError {
        get {
            return _hasError;
        }
        protected internal set {
            _hasError = value;
            if (_hasError) {
                if (ThrowException) {
                    throw new Exception(Common.GetListAsString(Connector._ReturnMessages, Environment.NewLine));
                }
            }
        }
    }

    /// <summary>
    /// A list of messages generated in the library
    /// <para>This list is used as a return mechanism of actions performed in this library</para>
    /// </summary>
    public static ArrayList ReturnMessages {
        get {
            return _ReturnMessages;
        }
        set {
            _ReturnMessages = value;
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if a connection to the domino server has been established
    /// </summary>
    public bool isConnected {
        get {
            return _isConnected;
        }
    }

    /// <summary>
    /// Indicates if the connector has been initialized
    /// <para>When not initialized no other methods can be executed</para>
    /// </summary>
    public bool isInitialized {
        get {
            return _isInitialized;
        }
    }

   

    /// <summary>
    /// A custom identification header, used when sending request to the domino database
    /// </summary>
    public string XPIdentity {
        get {
            return _XPIdentity;
        }

        set {
            _XPIdentity = value;
        }
    }

    /// <summary>
    /// Encryption IV Key
    /// </summary>
    protected internal string EncryptionIV {
        get {
            return _EncryptionIV;
        }

        set {
            _EncryptionIV = value;
        }
    }

    /// <summary>
    /// Encryption Password
    /// </summary>
    protected internal string EncryptionPASSWORD {
        get {
            return _EncryptionPASSWORD;
        }

        set {
            _EncryptionPASSWORD = value;
        }
    }

    /// <summary>
    /// Encryption Salt Key
    /// </summary>
    protected internal string EncryptionSALT {
        get {
            return _EncryptionSALT;
        }

        set {
            _EncryptionSALT = value;
        }
    }

    /// <summary>
    /// A SessionObject is used to establish a connection to the XPages database that hosts the Rest Service
    /// <para>In order to get the SessionObject, the user needs to have access to the XPages database</para>
    /// </summary>
    public SessionObject SessionObject {
        get {
            return _SessionObject;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the Connector and provides the possibility to use custom encryption keys and Identification header
    /// </summary>
    /// <param name="EncryptionPassword"></param>
    /// <param name="EncryptionIV"></param>
    /// <param name="EncryptionSalt"></param>
    /// <param name="Identify"></param>
    public void Initialize(string EncryptionPassword = null, string EncryptionIV = null, string EncryptionSalt = null, string Identify = null) {
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
            _isInitialized = true;
        } else {
            _isInitialized = false;
        }
    }

    /// <summary>
    /// When the connector has been initialized, this method will establish a connection to the domino server using the provided credentials
    /// <para>Sets isConnected property</para>
    /// </summary>
    /// <returns>boolean</returns>
    public bool Connect() {
        // only try to connect if initialized - all info needed is available
        if (_isInitialized) {
            ResetReturn();
            // reset the error vars - will be populated by Requestor
            Request = new Requestor(this);
            // creates connection to domino and tries to login with supplied credentials
            if (Request.Initialize()) {
                _isConnected = true;
                return true;
            } else {
                //unable to connect - hasError set to true & returnmessages added
                _isConnected = false;
                return false;
            }
        } else {
            //hasError is set to true (New()) and returnmessage contains reason
            _isConnected = false;
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
        if (_isConnected) {
            //we have a valid Requestor
            if (Request != null && Request.isInitialized) {
                SessionObject sObj = new SessionObject(this, DominoWebServiceURL);
                if (sObj.Initialize()) {
                    _SessionObject = sObj;
                    return sObj;
                    //not written in initialize - just returning false
                } else {
                    ReturnMessages.Add("Connector.GetSession unable to complete, unable to initialize the session object");
                    hasError = true;
                    //throws exception
                    return null;
                }
            } else {
                ReturnMessages.Add("Connector.GetSession unable to complete, the request object is invalid or not initialized");
                hasError = true;
                //throws exception
                return null;
            }
        } else {
            ReturnMessages.Add("Connector.GetSession unable to complete, Connector is not connected to domino");
            hasError = true;  //throws exception
            return null;
        }

    }

    #endregion

    #region Internal Methods

    /// <summary>
    /// Internal method to reset the return mechanism
    /// </summary>
    static internal void ResetReturn() {
        hasError = false;
        ReturnMessages = new ArrayList();
    }

    #endregion

}
