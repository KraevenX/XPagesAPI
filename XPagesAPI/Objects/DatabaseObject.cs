using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An object representing a domino database object
/// </summary>
/// <example>The following is an example of the usage of a
/// <c>DatabaseObject</c>:
///   <code>
///    DatabaseObject dbObj = new DatabaseObject(filePath, dominoServerName,  sessionObject); // by FilePath
///    //  DatabaseObject dbObj = new DatabaseObject(sessionObject, replicationID, dominoServerName); // by ReplicationID
///    if(dbObj!=null &amp;&amp; dbObj.Initialize()){
///         // your code here...
///    }
///   </code>
/// </example>
public class DatabaseObject {

    #region Properties

    /// <summary>
    /// Reference to the session object that retrieve this database object
    /// </summary>
    public SessionObject Session { get; }

    /// <summary>
    /// Indicates is the DatabaseObject has been initialized
    /// </summary>
    public bool IsInitialized { get; private set; } = false;

    /// <summary>
    /// Database FilePath
    /// </summary>
    public string FilePath { get; protected internal set; } = "";

    /// <summary>
    /// Database ReplicationID
    /// </summary>
    public string ReplicationID { get; protected internal set; } = "";

    /// <summary>
    /// Database ServerName
    /// </summary>
    public string ServerName { get; protected internal set; } = "";

    /// <summary>
    /// Database Title
    /// </summary>
    public string Title { get; protected internal set; } = "";

    /// <summary>
    /// Database Template name
    /// </summary>
    public string Template { get; protected internal set; } = "";

    /// <summary>
    /// Database size (bytes, Kb, Mb, Gb, Tb)
    /// </summary>
    public string Size { get; protected internal set; } = "";

    /// <summary>
    /// Database URL
    /// </summary>
    public string Url { get; protected internal set; } = "";

    /// <summary>
    /// Collection of retrieved DocumentObjects stored in a dictionary with key universalID of the document
    /// </summary>
    public Dictionary<string, DocumentObject> Documents { get; protected internal set; } = null;

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Database constructor method
    /// </summary>
    /// <param name="filePath">The domino file path (root = data folder)</param>
    /// <param name="server">The server name abbreviated (ANTLN-TEST/ANTWERPEN/JacobsEngineering) </param>
    /// <param name="session"></param>
    public DatabaseObject(string filePath, string server, SessionObject session) {
        Session = session;
        FilePath = filePath;
        ServerName = server;
    }

    /// <summary>
    /// Database constructor method
    /// </summary>
    /// <param name="session"></param>
    /// <param name="replicationID"></param>
    /// <param name="server">The server name abbreviated (ANTLN-TEST/ANTWERPEN/JacobsEngineering) </param>
    public DatabaseObject(SessionObject session, string replicationID, string server) {
        Session = session;
        ReplicationID = replicationID;
        ServerName = server;
    }

    /// <summary>
    /// Database constructor method via session with initialization string.
    /// This will not reset the Connector ReturnMessages
    /// </summary>
    /// <param name="session"></param>
    /// <param name="initString"></param>
    internal DatabaseObject(SessionObject session, string initString) {
        if (session != null && session.IsInitialized) {
            Session = session;
            if (!String.IsNullOrEmpty(initString)) {
                // FilePath§ServerName§ReplicationID§Title§Template§Size§URL
                if (initString.Contains("§")) {
                    String[] ar = initString.Split(new[] { "§" }, StringSplitOptions.None);
                    if (ar != null && ar.Length > 0 && ar.Length == 7) {
                        FilePath = ar[0];
                        ServerName = ar[1];
                        ReplicationID = ar[2];
                        Title = ar[3];
                        Template = ar[4];
                        Size = ar[5];
                        Url = ar[6];
                        IsInitialized = true;
                    } else {
                        Connector.ReturnMessages.Add("Unable to create database object - Initialization String Not Validated");
                        IsInitialized = false;
                        Connector.HasError = true;
                    }
                } else {
                    Connector.ReturnMessages.Add("Unable to create database object - Initialization String Not Valid");
                    IsInitialized = false;
                    Connector.HasError = true;
                }
            } else {
                Connector.ReturnMessages.Add("Unable to create database object - Initialization String Not Provided");
                IsInitialized = false;
                Connector.HasError = true;
            }
        } else {
            // session is not valid
            Connector.ReturnMessages.Add("Unable to create database object - Session Object Not Valid");
            IsInitialized = false;
            Connector.HasError = true;
        }
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    ///  Initializes the DatabaseObject by validating the input and triggering the database request
    /// </summary>
    /// <returns>Boolean</returns>
    public bool Initialize() {
        Connector.ResetReturn();
        // reset the list of docs
        Documents = null;

        if (!ValidateInput()) {
            IsInitialized = false;
            Connector.HasError = true;
            return false;   // throws exception
        }

        // make a connection to the webservice database - this will check the users authentication on that database
        if (Session.Connection.Request.ExecuteDatabaseRequest(Session.WebServiceURL, ServerName, FilePath, ReplicationID, this)) {
            IsInitialized = true;
            Connector.HasError = false;
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
        if (Session == null || !Session.IsInitialized) {
            //can only be initialized if valid connector and connector is initialized and connected
            Connector.ReturnMessages.Add("DatabaseObject can not be validated : Session is not initialized! (DatabaseObject.ValidateInput)");
            return false;
        }
        //check server, dbrep or filepath
        if (string.IsNullOrEmpty(ServerName)) {
            Connector.ReturnMessages.Add("DatabaseObject can not be validated : ServerName has not been provided! (DatabaseObject.ValidateInput)");
            return false;
        }

        if (string.IsNullOrEmpty(FilePath)) {
            //we must have a rep id
            if (string.IsNullOrEmpty(ReplicationID)) {
                Connector.ReturnMessages.Add("DatabaseObject can not be validated : FilePath/ReplicationID has not been provided! (DatabaseObject.ValidateInput)");
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Retrieve a document from this database by universalID
    /// <para>Triggers a document request</para>
    /// <para>The document will be added to the property 'Documents'</para>
    /// </summary>
    /// <param name="unid"></param>
    /// <returns>DocumentObject</returns>
    public DocumentObject GetDocument(string unid) {
        Connector.ResetReturn();
        DocumentObject docObj = null;
        if (IsInitialized) {
            docObj = new DocumentObject(this, unid);
            if (docObj.Initialize()) {
                return docObj;
            } else {
                return null;
            }
        }
        return docObj;
    }

    /// <summary>
    /// Retrieve a document from this database by universalID
    /// This function will, in addition to retrieving the default document information, retrieve the associated domino attachment file objects
    /// <para>Triggers a document request</para>
    /// <para>The document will be added to the property 'Documents'</para>
    /// </summary>
    /// <param name="unid"></param>
    /// <returns>DocumentObject</returns>
    public DocumentObject GetDocumentAndFiles(string unid) {
        Connector.ResetReturn();
        DocumentObject docObj = null;
        if (IsInitialized) {
            docObj = new DocumentObject(this, unid);
            if (docObj.InitializeWithFiles()) {
                return docObj;
            } else {
                return null;
            }
        }
        return docObj;
    }

    /// <summary>
    /// Retrieve a document from this database by searching for a specific value in the provided field.
    /// <para>The first document found will be returned</para>
    /// <para>Triggers a document request</para>
    /// <para>The document will be added to the property 'Documents'</para>
    /// </summary>
    /// <param name="searchField"></param>
    /// <param name="searchValue"></param>
    /// <returns>DocumentObject</returns>
    public DocumentObject GetDocumentByKey(string searchField, string searchValue) {
        Connector.ResetReturn();
        DocumentObject docObj = null;
        if (IsInitialized) {
            docObj = new DocumentObject(this, searchField, searchValue);
            if (docObj.Initialize()) {
                return docObj;
            } else {
                return null;
            }
        }
        return docObj;
    }

    /// <summary>
    /// Retrieve a document from this database by searching for a specific value in the provided field.
    /// This function will, in addition to retrieving the default document information, retrieve the associated domino attachment file objects
    /// <para>The first document found will be returned</para>
    /// <para>Triggers a document request</para>
    /// <para>The document will be added to the property 'Documents'</para>
    /// </summary>
    /// <param name="searchField"></param>
    /// <param name="searchValue"></param>
    /// <returns>DocumentObject</returns>
    public DocumentObject GetDocumentAndFilesByKey(string searchField, string searchValue) {
        Connector.ResetReturn();
        DocumentObject docObj = null;
        if (IsInitialized) {
            docObj = new DocumentObject(this, searchField, searchValue);
            if (docObj.InitializeWithFiles()) {
                return docObj;
            } else {
                return null;
            }
        }
        return docObj;
    }

    /// <summary>
    /// Retrieve a document from this database by searching using the provided formula.
    /// <para>The first document found will be returned</para>
    /// <para>Triggers a document request</para>
    /// <para>The document will be added to the property 'Documents'</para>
    /// </summary>
    /// <param name="formula"></param>
    /// <returns>DocumentObject</returns>
    public DocumentObject GetDocumentByFormula(string formula) {
        Connector.ResetReturn();
        DocumentObject docObj = null;
        if (IsInitialized) {
            docObj = new DocumentObject(formula, this);
            if (docObj.Initialize()) {
                return docObj;
            } else {
                return null;
            }
        }
        return docObj;
    }

    /// <summary>
    /// Retrieve a document from this database by searching using the provided formula.
    /// This function will, in addition to retrieving the default document information, retrieve the associated domino attachment file objects
    /// <para>The first document found will be returned</para>
    /// <para>Triggers a document request</para>
    /// <para>The document will be added to the property 'Documents'</para>
    /// </summary>
    /// <param name="formula"></param>
    /// <returns>DocumentObject</returns>
    public DocumentObject GetDocumentAndFilesByFormula(string formula) {
        Connector.ResetReturn();
        DocumentObject docObj = null;
        if (IsInitialized) {
            docObj = new DocumentObject(formula, this);
            if (docObj.InitializeWithFiles()) {
                return docObj;
            } else {
                return null;
            }
        }
        return docObj;
    }

    /// <summary>
    /// Retrieve all documents from the database
    /// <para>Triggers an all documents request</para>
    /// <para>All documents found will be added to the property 'Documents'</para>
    /// </summary>
    /// <returns>Boolean</returns>
    public bool GetAllDocuments() {
        Connector.ResetReturn();
        if (IsInitialized) {
            if (Session.Connection.Request.ExecuteAllDocumentsRequest(Session.WebServiceURL, null, null, null, null, this)) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Retrieve all documents from the database
    /// This function will, in addition to retrieving the default document information, retrieve the associated domino attachment file objects
    /// <para>Triggers an all documents request</para>
    /// <para>All documents found will be added to the property 'Documents'</para>
    /// </summary>
    /// <returns>Boolean</returns>
    public bool GetAllDocumentsAndFiles() {
        Connector.ResetReturn();
        if (IsInitialized) {
            if (Session.Connection.Request.ExecuteAllDocumentsFilesRequest(Session.WebServiceURL, null, null, null, null, this)) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Retrieve all documents from this database that correspond to the provided formula
    /// <para>Triggers an all documents request</para>
    /// <para>All documents found will be added to the property 'Documents'</para>
    /// </summary>
    /// <param name="formula"></param>
    /// <returns>Boolean</returns>
    public bool GetAllDocumentsByFormula(string formula) {
        Connector.ResetReturn();
        if (IsInitialized) {
            if (Session.Connection.Request.ExecuteAllDocumentsRequest(Session.WebServiceURL, null, null, formula, null, this)) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Retrieve all documents from this database that correspond to the provided formula
    /// This function will, in addition to retrieving the default document information, retrieve the associated domino attachment file objects
    /// <para>Triggers an all documents request</para>
    /// <para>All documents found will be added to the property 'Documents'</para>
    /// </summary>
    /// <param name="formula"></param>
    /// <returns>Boolean</returns>
    public bool GetAllDocumentsAndFilesByFormula(string formula) {
        Connector.ResetReturn();
        if (IsInitialized) {
            if (Session.Connection.Request.ExecuteAllDocumentsFilesRequest(Session.WebServiceURL, null, null, formula, null, this)) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Retrieve all documents from this database by universalIDs
    /// <para> Provide the universalIds separated by ;</para>
    /// <para>Triggers an all documents request</para>
    /// <para>All documents found will be added to the property 'Documents'</para>
    /// </summary>
    /// <param name="unids"></param>
    /// <returns>Boolean</returns>
    public bool GetAllDocumentsByUnids(string unids) {
        Connector.ResetReturn();
        if (IsInitialized) {
            if (Session.Connection.Request.ExecuteAllDocumentsRequest(Session.WebServiceURL, null, null, null, unids, this)) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Retrieve all documents from this database by universalIDs
    /// This function will, in addition to retrieving the default document information, retrieve the associated domino attachment file objects
    /// <para> Provide the universalIds separated by ;</para>
    /// <para>Triggers an all documents request</para>
    /// <para>All documents found will be added to the property 'Documents'</para>
    /// </summary>
    /// <param name="unids"></param>
    /// <returns>Boolean</returns>
    public bool GetAllDocumentsAndFilesByUnids(string unids) {
        Connector.ResetReturn();
        if (IsInitialized) {
            if (Session.Connection.Request.ExecuteAllDocumentsFilesRequest(Session.WebServiceURL, null, null, null, unids, this)) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Retrieve all documents from this database by universalIDs
    /// <para>Triggers an all documents request</para>
    /// <para>All documents found will be added to the property 'Documents'</para>
    /// </summary>
    /// <param name="listUnids"></param>
    /// <returns>Boolean</returns>
    public bool GetAllDocumentsByUnids(IList listUnids) {
        Connector.ResetReturn();
        if (IsInitialized) {
            string unids = Common.GetListAsString(listUnids, ";");

            if (Session.Connection.Request.ExecuteAllDocumentsRequest(Session.WebServiceURL, null, null, null, unids, this)) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Retrieve all documents from this database by universalIDs
    /// This function will, in addition to retrieving the default document information, retrieve the associated domino attachment file objects
    /// <para>Triggers an all documents request</para>
    /// <para>All documents found will be added to the property 'Documents'</para>
    /// </summary>
    /// <param name="listUnids"></param>
    /// <returns>Boolean</returns>
    public bool GetAllDocumentsAndFilesByUnids(IList listUnids) {
        Connector.ResetReturn();
        if (IsInitialized) {
            string unids = Common.GetListAsString(listUnids, ";");

            if (Session.Connection.Request.ExecuteAllDocumentsFilesRequest(Session.WebServiceURL, null, null, null, unids, this)) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Retrieve all documents from this database by searching for a specific value in the provided field
    /// <para>Triggers an all documents request</para>
    /// <para>All documents found will be added to the property 'Documents'</para>
    /// </summary>
    /// <param name="searchField"></param>
    /// <param name="searchValue"></param>
    /// <returns>Boolean</returns>
    public bool GetAllDocumentsByKey(string searchField, string searchValue) {
        Connector.ResetReturn();
        if (IsInitialized) {
            if (Session.Connection.Request.ExecuteAllDocumentsRequest(Session.WebServiceURL, searchField, searchValue, null, null, this)) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Retrieve all documents from this database by searching for a specific value in the provided field
    /// This function will, in addition to retrieving the default document information, retrieve the associated domino attachment file objects
    /// <para>Triggers an all documents request</para>
    /// <para>All documents found will be added to the property 'Documents'</para>
    /// </summary>
    /// <param name="searchField"></param>
    /// <param name="searchValue"></param>
    /// <returns>Boolean</returns>
    public bool GetAllDocumentsAndFilesByKey(string searchField, string searchValue) {
        Connector.ResetReturn();
        if (IsInitialized) {
            if (Session.Connection.Request.ExecuteAllDocumentsFilesRequest(Session.WebServiceURL, searchField, searchValue, null, null, this)) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Retrieve fields for all documents in the property 'Documents'
    /// <para>Provide the fields separated by ;</para>
    /// <para>This action will update the property collection Fields inside each document object</para>
    /// </summary>
    /// <param name="listFields"></param>
    /// <returns>Boolean</returns>
    public bool LoadDocumentFields(string listFields) {
        Connector.ResetReturn();
        if (IsInitialized && Documents != null && Documents.Count > 0) {
            if (Session.Connection.Request.ExecuteAllFieldsRequest(Session.WebServiceURL, this, listFields)) {
                return true;
            } else {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Retrieve fields for all documents in the property 'Documents'
    /// <para>This action will update the property collection Fields inside each document object</para>
    /// </summary>
    /// <param name="listFields"></param>
    /// <returns>Boolean</returns>
    public bool LoadDocumentFields(IList listFields) {
        Connector.ResetReturn();
        if (IsInitialized && Documents != null && Documents.Count > 0) {
            string fields = Common.GetListAsString(listFields, ";");
            if (Session.Connection.Request.ExecuteAllFieldsRequest(Session.WebServiceURL, this, fields)) {
                return true;
            } else {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Retrieve file objects for all documents in the property 'Documents'
    /// <para>This action will update the property collection Files inside each document object</para>
    /// </summary>
    /// <returns>Boolean</returns>
    public bool LoadDocumentFiles() {
        Connector.ResetReturn();
        if (IsInitialized && Documents != null && Documents.Count > 0) {
            if (Session.Connection.Request.ExecuteAllFilesRequest(Session.WebServiceURL, this)) {
                return true;
            } else {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Remove all retrieved documents from the 'Documents' property
    /// </summary>
    public void ClearDocuments() {
        Documents = null;
    }

    #endregion Methods
}