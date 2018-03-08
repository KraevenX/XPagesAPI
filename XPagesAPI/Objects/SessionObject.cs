#pragma warning disable CS1570 // XML comment has badly formed XML
                              /// <summary>
                              /// An object representing a Domino Session
                              /// </summary>
                              /// <example>The following is an example of the usage of a
                              /// <c>SessionObject</c>:
                              ///   <code>
                              ///    SessionObject sObj = new SessionObject(connectorObj, DominoWebServiceURL); // the connector object and Full URL to the XPage database containing the Web Rest Service
                              ///    
                              ///    if(sObj!=null && sObj.Initialize()){
                              ///         // here you can then get the database object
                              ///         // your code here... 
                              ///    }
                              ///   </code>
                              /// </example>
public class SessionObject
{
#pragma warning restore CS1570 // XML comment has badly formed XML

    #region Variables

    private Connector _Connection;
    private string _WebServiceURL;
    private bool _isInitialized = false;

    #endregion

    #region Constructors

    /// <summary>
    /// SessionObject Constructor method
    /// </summary>
    /// <param name="ConnectorObject"></param>
    /// <param name="DominoWebServiceURL"></param>
    public SessionObject(Connector ConnectorObject, string DominoWebServiceURL) {
        _Connection = ConnectorObject;
        _WebServiceURL = DominoWebServiceURL;
    }


    #endregion

    #region Properties

    /// <summary>
    /// Indicates if the session has been intialized
    /// </summary>
    public bool IsInitialized {
        get {
            return _isInitialized;
        }
    }

    /// <summary>
    /// XPages Rest Serive URL
    /// Example: http://antln-test.europe.jacobs.com/projects/jpix/Interface.nsf/xpJPIService.xsp/JPIService
    /// </summary>
    public string WebServiceURL {
        get {
            return _WebServiceURL;
        }
    }

    /// <summary>
    /// Reference to Connector
    /// </summary>
    public Connector Connection {
        get {
            return _Connection;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes the SessionObject by validating the input and triggering the session request
    /// <para>Sets isInitialized property</para>
    /// </summary>
    /// <returns>Boolean</returns>
    public bool Initialize() {
        Connector.ResetReturn();
        //clear msgs

        if (!ValidateInput()) {
            //message written trigger exception
            _isInitialized = false;
            Connector.hasError = true;
            return false;
        }

        // make a connection to the webservice database - this will check the users authentication on that database
        if (Connection.Request.ExecuteSessionRequest(WebServiceURL)) {
            Connector.ReturnMessages.Add("Session Initialized : " + WebServiceURL + " (SessionObject.Initialize)");
            Connector.hasError = false;
            _isInitialized = true;
            return true;
        } else {
            //error messages written to Connection.ReturnMessages by Connection.Request.ExecuteSessionRequest
            _isInitialized = false;
            return false;
        }

    }

    /// <summary>
    /// Validate the input provided by the user
    /// </summary>
    /// <returns>Boolean</returns>
    private bool ValidateInput() {

        if (_Connection != null && _WebServiceURL != null) {
            if (_Connection.isInitialized & _Connection.isConnected) {
                //only when we already have a connection (user is authenticated)
                if (WebServiceURL.ToLower().Contains("http://") || WebServiceURL.ToLower().Contains("https://")) {
                    //Connector.hasError = False
                    return true;
                } else {
                    Connector.ReturnMessages.Add("SessionObject is invalid : Web Service Url is not valid, http:// or https:// needs to be included (SessionObject.ValidateInput)");

                    // Connector.hasError = True 'throws exception
                    return false;
                }
            } else {
                Connector.ReturnMessages.Add("Connector Object not initialized or connected! (SessionObject.ValidateInput)");
                return false;
                // Connector.hasError = True 'throws exception
            }
        } else {
            Connector.ReturnMessages.Add("SessionObject is invalid : Connector or Web ServiceUrl is nothing! (SessionObject.ValidateInput)");
            // Connector.hasError = True 'throws exception
            return false;
        }
    }

    /// <summary>
    /// Method to retrieve a specific database by filepath on the given server
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="serverName"></param>
    /// <returns>DatabaseObject</returns>
    public DatabaseObject GetDatabase(string filePath, string serverName) {
        DatabaseObject dbObj = null;
        if (_isInitialized) {
            dbObj = new DatabaseObject(filePath, serverName, this);
            if (dbObj.Initialize()) {
                return dbObj;
            }
        }
        return dbObj;
    }

    /// <summary>
    /// Method to retrieve a specific database by replicationID on the given server
    /// </summary>
    /// <param name="replicationID"></param>
    /// <param name="serverName"></param>
    /// <returns></returns>
    public DatabaseObject GetDatabaseByID(string replicationID, string serverName) {
        DatabaseObject dbObj = null;
        if (_isInitialized) {
            dbObj = new DatabaseObject(this, replicationID, serverName);
            if (dbObj.Initialize()) {
                return dbObj;
            }
        }
        return dbObj;
    }

    #endregion

}
