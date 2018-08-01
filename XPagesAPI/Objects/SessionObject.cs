using System.Collections.Generic;
/// <summary>
/// An object representing a Domino Session
/// </summary>
/// <example>The following is an example of the usage of a
/// <c>SessionObject</c>:
///   <code>
///    SessionObject sObj = new SessionObject(connectorObj, DominoWebServiceURL); // the connector object and Full URL to the XPage database containing the Web Rest Service
///    
///    if(sObj!=null &amp;&amp; sObj.Initialize()){
///         // here you can then get the database object
///         // your code here... 
///    }
///   </code>
/// </example>
public class SessionObject
{

    #region Constructors

    /// <summary>
    /// SessionObject Constructor method
    /// </summary>
    /// <param name="ConnectorObject"></param>
    /// <param name="DominoWebServiceURL"></param>
    public SessionObject(Connector ConnectorObject, string DominoWebServiceURL) {
        Connection = ConnectorObject;
        WebServiceURL = DominoWebServiceURL;
    }


    #endregion

    #region Properties

    /// <summary>
    /// Indicates if the session has been intialized
    /// </summary>
    public bool IsInitialized { get; private set; } = false;

    /// <summary>
    /// XPages Rest Serive URL
    /// Example: http://antln-test.europe.jacobs.com/projects/jpix/Interface.nsf/xpJPIService.xsp/JPIService
    /// </summary>
    public string WebServiceURL { get; }

    /// <summary>
    /// Reference to Connector
    /// </summary>
    public Connector Connection { get; }

    /// <summary>
    ///  Collection of retrieved DatabaseObjects stored in a dictionary with key filepath of the database
    /// </summary>
    public SortedDictionary<string, DatabaseObject> Databases { get; protected internal set; }
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
            IsInitialized = false;
            Connector.hasError = true;
            return false;
        }

        // make a connection to the webservice database - this will check the users authentication on that database
        if (Connection.Request.ExecuteSessionRequest(WebServiceURL)) {
            Connector.ReturnMessages.Add("Session Initialized : " + WebServiceURL + " (SessionObject.Initialize)");
            Connector.hasError = false;
            IsInitialized = true;
            return true;
        } else {
            //error messages written to Connection.ReturnMessages by Connection.Request.ExecuteSessionRequest
            IsInitialized = false;
            return false;
        }

    }

    /// <summary>
    /// Validate the input provided by the user
    /// </summary>
    /// <returns>Boolean</returns>
    private bool ValidateInput() {

        if (Connection != null && WebServiceURL != null) {
            if (Connection.isInitialized & Connection.isConnected) {
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
        if (IsInitialized) {
            dbObj = new DatabaseObject(filePath, serverName, this);
            if (dbObj.Initialize()) {
                if (Databases == null) {
                    Databases = new SortedDictionary<string, DatabaseObject>();
                }
                if (Databases.ContainsKey(dbObj.FilePath)) {
                    //remove this and replace we newly retrieved database
                    Databases.Remove(dbObj.FilePath);
                }
                Databases.Add(dbObj.FilePath, dbObj);
                return dbObj;
            }
        }
        return dbObj;
    }

    /// <summary>
    /// Method to retrieve all databases from a given server
    /// </summary>
    /// <param name="serverName"></param>
    /// <returns>DatabaseObject</returns>
    public bool GetAllDatabases(string serverName) {
        Connector.ResetReturn();
        if (IsInitialized) {
            if (this.Connection.Request.ExecuteAllDatabasesRequest(this.WebServiceURL, serverName, this)) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Method to retrieve a specific database by replicationID on the given server
    /// </summary>
    /// <param name="replicationID"></param>
    /// <param name="serverName"></param>
    /// <returns></returns>
    public DatabaseObject GetDatabaseByID(string replicationID, string serverName) {
        DatabaseObject dbObj = null;
        if (IsInitialized) {
            dbObj = new DatabaseObject(this, replicationID, serverName);
            if (dbObj.Initialize()) {
                if(Databases == null) {
                    Databases = new SortedDictionary<string, DatabaseObject>();
                }
                if (Databases.ContainsKey(dbObj.FilePath)) {
                    //remove this and replace we newly retrieved database
                    Databases.Remove(dbObj.FilePath);
                }
                Databases.Add(dbObj.FilePath, dbObj);
                return dbObj;
            }
        }
        return dbObj;
    }

    /// <summary>
    /// Remove all retrieved databases from the 'Databases' property
    /// </summary>
    public void ClearDatabases() {
        Databases = null;
    }

    #endregion

}
