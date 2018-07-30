using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
public class DocumentObject
{

    #region Variables

    private DatabaseObject _Database;

    private bool _isInitialized = false;
    private string _DateCreated = "";
    private string _DateModified = "";
    private string _UniversalID = "";
    private string _NoteID = "";
    private string _SearchField = "";
    private string _SearchValue = "";
    private string _Formula = "";
    private string _Size = "";
    private string _Url = "";
    private string _Form = "";
    private SortedDictionary<String, FieldObject> _Fields = null;
    private SortedDictionary<String, FileObject> _Files = null;

    #endregion

    #region Properties

    /// <summary>
    /// Reference to this documents database object
    /// </summary>
    public DatabaseObject Database {
        get {
            return _Database;
        }
    }

    /// <summary>
    /// Indicates that the document isInitialized
    /// </summary>
    public bool IsInitialized {
        get {
            return _isInitialized;
        }
        protected internal set {
            _isInitialized = value;
        }
    }

    /// <summary>
    /// Document Size
    /// </summary>
    public string Size {
        get {
            return _Size;
        }

        protected internal set {
            _Size = value;
        }
    }

    /// <summary>
    /// Document URL
    /// </summary>
    public string Url {
        get {
            return _Url;
        }

        protected internal set {
            _Url = value;
        }
    }

    /// <summary>
    /// Documents Form (if any)
    /// </summary>
    public string Form {
        get {
            return _Form;
        }

        protected internal set {
            _Form = value;
        }
    }

    /// <summary>
    /// Document NoteId
    /// </summary>
    public string NoteID {
        get {
            return _NoteID;
        }

        protected internal set {
            _NoteID = value;
        }
    }

    /// <summary>
    /// Collection of retrieved FieldObjects stored in a dictionary with key name of the field
    /// </summary>
    public SortedDictionary<string, FieldObject> Fields {
        get {
            return _Fields;
        }

        protected internal set {
            _Fields = value;
        }
    }

    /// <summary>
    /// Collection of retrieved FileObjects stored in a dictionary with key filename of the attachment
    /// </summary>
    public SortedDictionary<string, FileObject> Files {
        get {
            return _Files;
        }

        protected internal set {
            _Files = value;
        }
    }

    /// <summary>
    /// Document UniversalID
    /// </summary>
    public string UniversalID {
        get {
            return _UniversalID;
        }

        protected internal set {
            _UniversalID = value;
        }
    }

    /// <summary>
    /// Document Creation Date
    /// </summary>
    public string DateCreated {
        get {
            return _DateCreated;
        }

        set {
            _DateCreated = value;
        }
    }

    /// <summary>
    /// Document Modification Date
    /// </summary>
    public string DateModified {
        get {
            return _DateModified;
        }

        set {
            _DateModified = value;
        }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// DocumentObject Constructor
    /// </summary>
    /// <param name="dbObj"></param>
    /// <param name="UniversalID"></param>
    public DocumentObject(DatabaseObject dbObj, String UniversalID)
    {
        _Database = dbObj;
        _UniversalID = UniversalID;
    }

    /// <summary>
    /// DocumentObject Constructor
    /// </summary>
    /// <param name="dbObj"></param>
    /// <param name="SeachField"></param>
    /// <param name="SearchValue"></param>
    public DocumentObject(DatabaseObject dbObj, string SeachField, string SearchValue)
    {
        _Database = dbObj;
        _SearchField = SeachField;
        _SearchValue = SearchValue;
    }

    /// <summary>
    /// DocumentObject Constructor
    /// </summary>
    /// <param name="formula"></param>
    /// <param name="dbObj"></param>
    public DocumentObject(string formula, DatabaseObject dbObj)
    {
        _Database = dbObj;
        _Formula = formula;
    }

    #endregion

    #region Private Methods

    private bool ValidateInput()
    {
        if (_Database == null || !_Database.IsInitialized)
        {
            //can only be initialized if valid connector and connector is initialized and connected, and session is initialized & databaseobject isInitialized
            Connector.ReturnMessages.Add("DocumentObject can not be validated : Database is not initialized! (DocumentObject.ValidateInput)");
            return false;
        }

        if (string.IsNullOrEmpty(_UniversalID))
        {
            //can be empty if we are searching by key
            if (string.IsNullOrEmpty(_SearchField) && string.IsNullOrEmpty(_SearchValue))
            {
                //search by formula?
                if (string.IsNullOrEmpty(_Formula))
                {
                    Connector.ReturnMessages.Add("DocumentObject can not be validated : UniversalID, SearchField, SearchValue or Formula have not been provided! (DocumentObject.ValidateInput)");
                    return false;
                }
            }
        }
        return true;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the document by validating the input and triggering the document request
    /// </summary>
    /// <returns>Boolean</returns>
    public bool Initialize()
    {

        Connector.ResetReturn();

        if (!ValidateInput())
        {
            _isInitialized = false;
            Connector.hasError = true;
            return false;   // throws exception
        }

        // make a connection to the webservice database - this will check the users authentication on that database
        if (_Database.Session.Connection.Request.ExecuteDocumentRequest(_Database.Session.WebServiceURL, _UniversalID, _SearchField, _SearchValue, _Formula, _Database, this))
        {
            _isInitialized = true;
            //  Connector.ReturnMessages.Add("Document Initialized : " + _UniversalID + " in : " + _Database.FilePath + " from : "+_Database.ServerName + " (DocumentObject.Initialize)");
            Connector.hasError = false;
            return true;
        }
        else
        {
            //error messages written to Connection.ReturnMessages by Connection.Request.ExecuteSessionRequest
            _isInitialized = false;
            return false;
        }
    }

    /// <summary>
    /// Initializes the document by validating the input and triggering the document request
    /// This function will, in addition to retrieving the default document information, retrieve the associated domino attachment file objects
    /// </summary>
    /// <returns>Boolean</returns>
    public bool InitializeWithFiles()
    {

        Connector.ResetReturn();

        if (!ValidateInput())
        {
            _isInitialized = false;
            Connector.hasError = true;
            return false;   // throws exception
        }

        // make a connection to the webservice database - this will check the users authentication on that database
        if (_Database.Session.Connection.Request.ExecuteDocumentFilesRequest(_Database.Session.WebServiceURL, _UniversalID, _SearchField, _SearchValue, _Formula, _Database, this))
        {
            _isInitialized = true;
            //  Connector.ReturnMessages.Add("Document Initialized : " + _UniversalID + " in : " + _Database.FilePath + " from : "+_Database.ServerName + " (DocumentObject.Initialize)");
            Connector.hasError = false;
            return true;
        }
        else
        {
            //error messages written to Connection.ReturnMessages by Connection.Request.ExecuteSessionRequest
            _isInitialized = false;
            return false;
        }
    }

    /// <summary>
    /// Retrieve fields for this documents by triggering the field request
    /// <para>Provide the fields separated by ;</para>
    /// <para>This action will update the property 'Fields'</para>
    /// </summary>
    /// <param name="fields"></param>
    /// <returns>Boolean</returns>
    public bool GetFields(string fields)
    {
        Connector.ResetReturn();

        if (_isInitialized)
        {
            // if (!String.IsNullOrEmpty(fields)) {
            //this will update _Fields list in this object
            if (_Database.Session.Connection.Request.ExecuteFieldsRequest(_Database.Session.WebServiceURL, this, fields))
            {
                Connector.hasError = false;
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
    public bool GetFiles()
    {
        Connector.ResetReturn();
        // get the files from XPages
        if (_isInitialized)
        {

            //this will update _Fields list in this object
            //string WebServiceURL, string Unid, string searchField, string searchValue, string formula, DatabaseObject dbObj, DocumentObject docObj
            if (_Database.Session.Connection.Request.ExecuteDocumentFilesRequest(_Database.Session.WebServiceURL, this._UniversalID,this._SearchField,this._SearchValue, this._Formula,this._Database,this))
            {
                Connector.hasError = false;
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
    public bool GetFields(IList fields)
    {
        Connector.ResetReturn();
        string f = "";
        if (_isInitialized)
        {

            f = Common.GetListAsString(fields, ";"); // can be empty - get's all fields
            // if (!String.IsNullOrEmpty(fields)) {
            //this will update _Fields list in this object
            if (_Database.Session.Connection.Request.ExecuteFieldsRequest(_Database.Session.WebServiceURL, this, f))
            {
                Connector.hasError = false;
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
    public bool GetAllFields()
    {
        //get all the fields from the domino document and store them in Fields
        Connector.ResetReturn();

        if (_isInitialized)
        {

            if (_Database.Session.Connection.Request.ExecuteFieldsRequest(_Database.Session.WebServiceURL, this, null))
            {
                Connector.hasError = false;
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
    public bool ExportFields(string filePath, bool overwriteExistingFile)
    {
        Connector.ResetReturn();
        try
        {
            if (Fields != null && Fields.Count > 0)
            {
                //write all to stringbuilder, output to file 
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, FieldObject> field in Fields)
                {
                    if (field.Value.Type.Equals("[FIELDNOTFOUND]"))
                    {
                        sb.AppendLine(field.Value.Name + " : " + field.Value.Type); // add type to indicate the field was not found
                    }
                    else
                    {
                        sb.AppendLine(field.Value.Name + " : " + field.Value.GetValueAsString()); //+ " (" + field.Value.Type + ")");
                    }
                }
                if (File.Exists(filePath))
                {
                    if (overwriteExistingFile)
                    {
                        File.Delete(filePath);
                    }
                    else
                    {
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
            }
            else
            {
                Connector.ReturnMessages.Add("The document : " + UniversalID + " does not contain any fields to export");
                return false;
            }

        }
        catch (Exception ex)
        {
            Connector.ReturnMessages.Add("Unable to export the fields to the file : " + filePath);
            Connector.ReturnMessages.Add(Common.GetErrorInfo(ex));
            Connector.hasError = true;
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
    public bool ExportFiles(string filePath, bool overwriteExistingFile)
    {
        Connector.ResetReturn();
        try
        {
            if (Files != null && Files.Count > 0)
            {
              
                if (!Directory.Exists(filePath))
                {
                    //create folde structure
                    Directory.CreateDirectory(filePath);
                }
                
                foreach(FileObject fObj in Files.Values)
                {
                    string fileName = "";
                    fileName = System.IO.Path.Combine(filePath,fObj.Name);
                    if (File.Exists(fileName))
                    {
                        if (overwriteExistingFile) {
                            File.Delete(fileName);
                        }
                        else
                        {
                            Connector.ReturnMessages.Add("Unable to export the file to : " + filePath);
                            Connector.ReturnMessages.Add("A file already exist with the same name, provide a new file path");
                            // Connector.hasError = true;
                            return false;
                        }
                      
                    }

                    if (!fObj.ExtractFile(fileName))
                    {
                        Connector.ReturnMessages.Insert(0,"Unable to export the file : " + fObj.Name);
                        Connector.hasError = true;
                        return false;
                    }
                    
                }

             
                Connector.ReturnMessages.Add("Exported all fields of document : " + UniversalID + " to the file : " + filePath);
                return true;
            }
            else
            {
                Connector.ReturnMessages.Add("The document : " + UniversalID + " does not contain any fields to export");
                return false;
            }

        }
        catch (Exception ex)
        {
            Connector.ReturnMessages.Add("Unable to export the fields to the file : " + filePath);
            Connector.ReturnMessages.Add(Common.GetErrorInfo(ex));
            Connector.hasError = true;
            return false;
        }
    }

    #endregion

}
