using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// An object representing a Domino Document
/// </summary>
/// <example>The following is an example of the usage of a
/// <c>DocumentObject</c>:
///   <code>
///    DocumentObject docObj = new DocumentObject(dbObj, universalID); // create new doc object by databaseobject and universalID
///    // DocumentObject docObj = new DocumentObject(dbObj, SeachField, SearchValue); // create new doc object by databaseobject and supplying a searchfield/value
///    // DocumentObject docObj = new DocumentObject(formula, dbObj); // create new doc object by databaseobject and supplying a formula
///    if(docObj!=null &amp;&amp; docObj.Initialize()){
///         // here you can then get the fields of the document object
///         // your code here...
///    }
///   </code>
/// </example>
public class DocumentObject {

    #region Variables

    private readonly string _SearchField = "";
    private readonly string _SearchValue = "";
    private readonly string _Formula = "";

    #endregion Variables

    #region Properties

    /// <summary>
    /// Reference to this documents database object
    /// </summary>
    public DatabaseObject Database { get; }

    /// <summary>
    /// Indicates that the document isInitialized
    /// </summary>
    public bool IsInitialized { get; protected internal set; } = false;

    /// <summary>
    /// Document Size
    /// </summary>
    public string Size { get; protected internal set; } = "";

    /// <summary>
    /// Document URL
    /// </summary>
    public string Url { get; protected internal set; } = "";

    /// <summary>
    /// Documents Form (if any)
    /// </summary>
    public string Form { get; protected internal set; } = "";

    /// <summary>
    /// Document NoteId
    /// </summary>
    public string NoteID { get; protected internal set; } = "";

    /// <summary>
    /// Collection of retrieved FieldObjects stored in a dictionary with key name of the field
    /// </summary>
    public SortedDictionary<string, FieldObject> Fields { get; protected internal set; } = null;

    /// <summary>
    /// Collection of retrieved FileObjects stored in a dictionary with key filename of the attachment
    /// </summary>
    public SortedDictionary<string, FileObject> Files { get; protected internal set; } = null;

    /// <summary>
    /// Document UniversalID
    /// </summary>
    public string UniversalID { get; protected internal set; } = "";

    /// <summary>
    /// Document Creation Date
    /// </summary>
    public string DateCreated { get; set; } = "";

    /// <summary>
    /// Document Modification Date
    /// </summary>
    public string DateModified { get; set; } = "";

    #endregion Properties

    #region Constructors

    /// <summary>
    /// DocumentObject Constructor
    /// </summary>
    /// <param name="dbObj"></param>
    /// <param name="UniversalID"></param>
    public DocumentObject(DatabaseObject dbObj, String UniversalID) {
        Database = dbObj;
        this.UniversalID = UniversalID;
    }

    /// <summary>
    /// DocumentObject Constructor
    /// </summary>
    /// <param name="dbObj"></param>
    /// <param name="SeachField"></param>
    /// <param name="SearchValue"></param>
    public DocumentObject(DatabaseObject dbObj, string SeachField, string SearchValue) {
        Database = dbObj;
        _SearchField = SeachField;
        _SearchValue = SearchValue;
    }

    /// <summary>
    /// DocumentObject Constructor
    /// </summary>
    /// <param name="formula"></param>
    /// <param name="dbObj"></param>
    public DocumentObject(string formula, DatabaseObject dbObj) {
        Database = dbObj;
        _Formula = formula;
    }

    #endregion Constructors

    #region Private Methods

    private bool ValidateInput() {
        if (Database == null || !Database.IsInitialized) {
            //can only be initialized if valid connector and connector is initialized and connected, and session is initialized & databaseobject isInitialized
            Connector.ReturnMessages.Add("DocumentObject can not be validated : Database is not initialized! (DocumentObject.ValidateInput)");
            return false;
        }

        if (string.IsNullOrEmpty(UniversalID)) {
            //can be empty if we are searching by key
            if (string.IsNullOrEmpty(_SearchField) && string.IsNullOrEmpty(_SearchValue)) {
                //search by formula?
                if (string.IsNullOrEmpty(_Formula)) {
                    Connector.ReturnMessages.Add("DocumentObject can not be validated : UniversalID, SearchField, SearchValue or Formula have not been provided! (DocumentObject.ValidateInput)");
                    return false;
                }
            }
        }
        return true;
    }

    #endregion Private Methods

    #region Public Methods

    /// <summary>
    /// Initializes the document by validating the input and triggering the document request
    /// </summary>
    /// <returns>Boolean</returns>
    public bool Initialize() {
        Connector.ResetReturn();

        // reset the lists
        Fields = null;
        Files = null;

        if (!ValidateInput()) {
            IsInitialized = false;
            Connector.HasError = true;
            return false;   // throws exception
        }

        // make a connection to the webservice database - this will check the users authentication on that database
        if (Database.Session.Connection.Request.ExecuteDocumentRequest(Database.Session.WebServiceURL, UniversalID, _SearchField, _SearchValue, _Formula, Database, this)) {
            IsInitialized = true;
            //  Connector.ReturnMessages.Add("Document Initialized : " + _UniversalID + " in : " + _Database.FilePath + " from : "+_Database.ServerName + " (DocumentObject.Initialize)");
            Connector.HasError = false;
            return true;
        } else {
            //error messages written to Connection.ReturnMessages by Connection.Request.ExecuteSessionRequest
            IsInitialized = false;
            return false;
        }
    }

    /// <summary>
    /// Initializes the document by validating the input and triggering the document request
    /// This function will, in addition to retrieving the default document information, retrieve the associated domino attachment file objects
    /// </summary>
    /// <returns>Boolean</returns>
    public bool InitializeWithFiles() {
        Connector.ResetReturn();

        if (!ValidateInput()) {
            IsInitialized = false;
            Connector.HasError = true;
            return false;   // throws exception
        }

        // make a connection to the webservice database - this will check the users authentication on that database
        if (Database.Session.Connection.Request.ExecuteDocumentFilesRequest(Database.Session.WebServiceURL, UniversalID, _SearchField, _SearchValue, _Formula, Database, this)) {
            IsInitialized = true;
            //  Connector.ReturnMessages.Add("Document Initialized : " + _UniversalID + " in : " + _Database.FilePath + " from : "+_Database.ServerName + " (DocumentObject.Initialize)");
            Connector.HasError = false;
            return true;
        } else {
            //error messages written to Connection.ReturnMessages by Connection.Request.ExecuteSessionRequest
            IsInitialized = false;
            return false;
        }
    }

    /// <summary>
    /// Initializes the document by validating the input and triggering the document request
    /// This function will, in addition to retrieving the default document information, retrieve the associated domino attachment file objects and all fields
    /// </summary>
    /// <returns>Boolean</returns>
    public bool InitializeWithFilesAndFields() {
        Connector.ResetReturn();

        if (!ValidateInput()) {
            IsInitialized = false;
            Connector.HasError = true;
            return false;   // throws exception
        }

        // make a connection to the webservice database - this will check the users authentication on that database
        if (Database.Session.Connection.Request.ExecuteDocumentFilesFieldsRequest(Database.Session.WebServiceURL, UniversalID, _SearchField, _SearchValue, _Formula, "", Database, this)) {
            IsInitialized = true;
            //  Connector.ReturnMessages.Add("Document Initialized : " + _UniversalID + " in : " + _Database.FilePath + " from : "+_Database.ServerName + " (DocumentObject.Initialize)");
            Connector.HasError = false;
            return true;
        } else {
            //error messages written to Connection.ReturnMessages by Connection.Request.ExecuteSessionRequest
            IsInitialized = false;
            return false;
        }
    }

    /// <summary>
    /// Initializes the document by validating the input and triggering the document request
    /// <para>This function will, in addition to retrieving the default document information, retrieve the associated domino attachment file objects and specific fields</para>
    /// <para>Provide the fields separated by ;</para>
    /// </summary>
    /// <param name="fields">Field Names separated by ; </param>
    /// <returns>bool</returns>
    public bool InitializeWithFilesAndFields(string fields) {
        Connector.ResetReturn();

        if (!ValidateInput()) {
            IsInitialized = false;
            Connector.HasError = true;
            return false;   // throws exception
        }

        // make a connection to the webservice database - this will check the users authentication on that database
        if (Database.Session.Connection.Request.ExecuteDocumentFilesFieldsRequest(Database.Session.WebServiceURL, UniversalID, _SearchField, _SearchValue, _Formula, fields, Database, this)) {
            IsInitialized = true;
            //  Connector.ReturnMessages.Add("Document Initialized : " + _UniversalID + " in : " + _Database.FilePath + " from : "+_Database.ServerName + " (DocumentObject.Initialize)");
            Connector.HasError = false;
            return true;
        } else {
            //error messages written to Connection.ReturnMessages by Connection.Request.ExecuteSessionRequest
            IsInitialized = false;
            return false;
        }
    }


    /// <summary>
    /// Retrieve fields for this documents by triggering the field request
    /// <para>Provide the fields separated by ;</para>
    /// <para>This action will update the property 'Fields'</para>
    /// </summary>
    /// <param name="fields"></param>
    /// <returns>bool</returns>
    public bool GetFields(string fields) {
        Connector.ResetReturn();

        if (IsInitialized) {
            // if (!String.IsNullOrEmpty(fields)) {
            //this will update _Fields list in this object
            if (Database.Session.Connection.Request.ExecuteFieldsRequest(Database.Session.WebServiceURL, this, fields)) {
                Connector.HasError = false;
                return true;
            }
            //}
        }
        return false;
        // get the fields from XPages
    }

    /// <summary>
    /// Retrieve file objects for this documents by triggering the document files request
    /// <para>This action will update the property 'Files'</para>
    /// </summary>
    /// <returns>Boolean</returns>
    public bool GetFiles() {
        Connector.ResetReturn();
        // get the files from XPages
        if (IsInitialized) {
            //this will update _Fields list in this object
            //string WebServiceURL, string Unid, string searchField, string searchValue, string formula, DatabaseObject dbObj, DocumentObject docObj
            if (Database.Session.Connection.Request.ExecuteDocumentFilesRequest(Database.Session.WebServiceURL, this.UniversalID, this._SearchField, this._SearchValue, this._Formula, this.Database, this)) {
                Connector.HasError = false;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Retrieve fields for this documents by triggering the field request
    /// <para>This action will update the property 'Fields'</para>
    /// </summary>
    /// <param name="fields"></param>
    /// <returns>Boolean</returns>
    public bool GetFields(IList fields) {
        Connector.ResetReturn();
        string f = "";
        if (IsInitialized) {
            f = Common.GetListAsString(fields, ";"); // can be empty - get's all fields
            // if (!String.IsNullOrEmpty(fields)) {
            //this will update _Fields list in this object
            if (Database.Session.Connection.Request.ExecuteFieldsRequest(Database.Session.WebServiceURL, this, f)) {
                Connector.HasError = false;
                return true;
            }
            //}
        }
        return false;
    }

    /// <summary>
    /// Retrieve all fields for this documents by triggering the all fields request
    /// <para> This action will update the property 'Fields' </para>
    /// </summary>
    /// <returns>Boolean</returns>
    public bool GetAllFields() {
        //get all the fields from the domino document and store them in Fields
        Connector.ResetReturn();

        if (IsInitialized) {
            if (Database.Session.Connection.Request.ExecuteFieldsRequest(Database.Session.WebServiceURL, this, null)) {
                Connector.HasError = false;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Exports the content of the property 'Fields' to a file
    /// <para>Provide the path to the file and specify if you want to overwrite this file if it exists</para>
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="overwriteExistingFile"></param>
    /// <returns></returns>
    public bool ExportFields(string filePath, bool overwriteExistingFile) {
        Connector.ResetReturn();
        try {
            if (Fields != null && Fields.Count > 0) {
                //write all to stringbuilder, output to file
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, FieldObject> field in Fields) {
                    if (field.Value.Type.Equals("[FIELDNOTFOUND]")) {
                        sb.AppendLine(field.Value.Name + " : " + field.Value.Type); // add type to indicate the field was not found
                    } else {
                        sb.AppendLine(field.Value.Name + " : " + field.Value.GetValueAsString()); //+ " (" + field.Value.Type + ")");
                    }
                }
                if (File.Exists(filePath)) {
                    if (overwriteExistingFile) {
                        File.Delete(filePath);
                    } else {
                        Connector.ReturnMessages.Add("Unable to export the fields to the file : " + filePath);
                        Connector.ReturnMessages.Add("A file already exist with the same name, provide a new file path");
                        // Connector.hasError = true;
                        return false;
                    }
                }
                //File.CreateText(filePath);
                File.WriteAllText(filePath, sb.ToString());
                Connector.ReturnMessages.Add("Exported all fields of document : " + UniversalID + " to the file : " + filePath);
                return true;
            } else {
                Connector.ReturnMessages.Add("The document : " + UniversalID + " does not contain any fields to export");
                return false;
            }
        } catch (Exception ex) {
            Connector.ReturnMessages.Add("Unable to export the fields to the file : " + filePath);
            Connector.ReturnMessages.Add(Common.GetErrorInfo(ex));
            Connector.HasError = true;
            return false;
        }
    }

    /// <summary>
    /// Exports the files from the XPages URL for all FileObjects in the Files property
    /// <para>Provide the path to the file and specify if you want to overwrite this file if it exists</para>
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="overwriteExistingFile"></param>
    /// <returns></returns>
    public bool ExportFiles(string filePath, bool overwriteExistingFile) {
        Connector.ResetReturn();
        try {
            if (Files != null && Files.Count > 0) {
                if (!Directory.Exists(filePath)) {
                    //create folde structure
                    Directory.CreateDirectory(filePath);
                }

                foreach (FileObject fObj in Files.Values) {
                    string fileName = "";
                    fileName = System.IO.Path.Combine(filePath, fObj.Name);
                    if (File.Exists(fileName)) {
                        if (overwriteExistingFile) {
                            File.Delete(fileName);
                        } else {
                            Connector.ReturnMessages.Add("Unable to export the file to : " + filePath);
                            Connector.ReturnMessages.Add("A file already exist with the same name, provide a new file path");
                            // Connector.hasError = true;
                            return false;
                        }
                    }

                    if (!fObj.ExtractFile(fileName)) {
                        Connector.ReturnMessages.Insert(0, "Unable to export the file : " + fObj.Name);
                        Connector.HasError = true;
                        return false;
                    }
                }

                Connector.ReturnMessages.Add("Exported all fields of document : " + UniversalID + " to the file : " + filePath);
                return true;
            } else {
                Connector.ReturnMessages.Add("The document : " + UniversalID + " does not contain any fields to export");
                return false;
            }
        } catch (Exception ex) {
            Connector.ReturnMessages.Add("Unable to export the fields to the file : " + filePath);
            Connector.ReturnMessages.Add(Common.GetErrorInfo(ex));
            Connector.HasError = true;
            return false;
        }
    }

    /// <summary>
    /// Remove all the retrieved field objects from this document
    /// </summary>
    public void ClearFields() {
        Fields = null;
    }

    /// <summary>
    /// Remove all the retrieved file objects from this document
    /// </summary>
    public void ClearFiles() {
        Files = null;
    }

    #endregion Public Methods
}