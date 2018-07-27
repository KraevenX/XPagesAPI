using System;
/// <summary>
/// An object representing a Domino File Attachment
/// </summary>
/// <example>The following is an example of the usage of a
/// <c>FileObject</c>:
///   <code>
///    FileObject fObj = new FileObject(docObj); // create new file object by providing the documentobject
///    if(fObj!=null &amp;&amp; fObj.Initialize(initializationString)){ 
///         // your code here... 
///    }
///   </code>
/// </example>
public class FileObject
{

    #region Variables

    private DocumentObject _Document;

    private bool _isInitialized = false;
    private string _Name = "";
    private string _URL = "";
    private long _Size;

    private string strSize = "";
    private string strDateCreated = "";
    private string strDateModified = "";

    private string _Application = "";
    private string _Creator = "";
    private DateTime _DateCreated;
    private DateTime _DateModified;
    private string _FieldName = "";
    private string _Extension = "";
    private string _Other = "";
    private string _SoftClass = "";
    //sb.append("FileObject: " + fObj.Application + "§" + fObj.Creator + "§" + fObj.DateCreated + "§" + fObj.DateModfied + "$" + fObj.FieldName + "§" + fObj.FileExtension + "§" + fObj.FileName + "$" + fObj.FileSize + "$" + fObj.LinkToFile + "$" + fObj.Other + "$"	+ fObj.SoftClass);

    #endregion

    #region Properties

    /// <summary>
    /// Reference to this files document object
    /// </summary>
    public DocumentObject Document {
        get {
            return _Document;
        }
    }
    /// <summary>
    /// The file name of the domino attachment
    /// </summary>
    public string Name { get => _Name; set => _Name = value; }

    /// <summary>
    /// URL of the domino attachment
    /// </summary>
    public string URL { get => _URL; set => _URL = value; }

    /// <summary>
    /// Size of the domino attachment in bytes
    /// </summary>
    public long Size { get => _Size; set => _Size = value; }

    /// <summary>
    /// JPI Soft Class of the domino attachment (Acad,ustn,office)
    /// </summary>
    public string SoftClass { get => _SoftClass; set => _SoftClass = value; }

    /// <summary>
    /// JPI associated application of the domino attachment
    /// </summary>
    public string Application { get => _Application; set => _Application = value; }

    /// <summary>
    /// Creator of the domino attachment
    /// </summary>
    public string Creator { get => _Creator; set => _Creator = value; }

    /// <summary>
    /// Creation date of the domino attachment
    /// </summary>
    public DateTime DateCreated { get => _DateCreated; set => _DateCreated = value; }

    /// <summary>
    /// Modified date of the domino attachment
    /// </summary>
    public DateTime DateModified { get => _DateModified; set => _DateModified = value; }

    /// <summary>
    /// FieldName (Rich-Text item) associated to the domino attachment if available
    /// </summary>
    public string FieldName { get => _FieldName; set => _FieldName = value; }

    /// <summary>
    /// The file extension of the domino attachment
    /// </summary>
    public string Extension { get => _Extension; set => _Extension = value; }

    /// <summary>
    /// Additional JPI information of the domino attachment
    /// </summary>
    public string Other { get => _Other; set => _Other = value; }

    /// <summary>
    /// Indicates that the file isInitialized
    /// </summary>
    public bool IsInitialized {
        get {
            return _isInitialized;
        }
        protected internal set {
            _isInitialized = value;
        }
    }
    #endregion

    #region Constructor

    /// <summary>
    /// FileObject Constructor 
    /// </summary>
    public FileObject(DocumentObject docObj)
    {
        _Document = docObj;
    }

    /// <summary>
    /// Initialize the file object, parses the initialization string into their respective properties
    /// </summary>
    /// <param name="initString"></param>
    /// <returns></returns>
    public bool Initialize(string initString)
    {
        //split the string into the properties here
        if (!String.IsNullOrEmpty(initString)){
            // fObj.Application + "§" + fObj.Creator + "§" + fObj.DateCreated + "§" + fObj.DateModfied + "$" + fObj.FieldName + "§" + fObj.FileExtension + "§" + 
            //fObj.FileName + "$" + fObj.FileSize + "$" + fObj.LinkToFile + "$" + fObj.Other + "$" + fObj.SoftClass);
            if (initString.Contains("§"))
            {
                String[] ar = initString.Split(new[] { "§" }, StringSplitOptions.None);
                if(ar != null && ar.Length > 0 && ar.Length == 11)
                {

                    _Application = ar[0];
                    _Creator = ar[1];
                    strDateCreated = ar[2];
                    if (!string.IsNullOrEmpty(strDateCreated) && !strDateCreated.Equals("."))
                    {
                        try
                        {
                            //convert to real datetime
                            _DateCreated = DateTime.Parse(strDateCreated);

                        }catch(Exception)
                        {
                            //do nothing or report?
                        }
                    }

                    strDateModified = ar[3];
                    if (!string.IsNullOrEmpty(strDateModified) && !strDateModified.Equals("."))
                    {
                        try
                        {
                            //convert to real datetime
                            _DateModified = DateTime.Parse(strDateModified);

                        }
                        catch (Exception)
                        {
                            //do nothing or report?
                        }
                    }

                    _FieldName = ar[4];
                    _Extension = ar[5];
                    _Name = ar[6];
                    strSize = ar[7];
                    if (!string.IsNullOrEmpty(strSize))
                    {
                        try
                        {
                            _Size = long.Parse(strSize);
                            _Size = _Size * 1024;
                        }
                        catch (Exception)
                        {
                            // throw;
                        }
                    }
                    _URL = ar[8];
                    _Other = ar[9];
                    _SoftClass = ar[10];

                    //validating can be done here if not all values are retrieved - atm see what we can get without failing

                    _isInitialized = true;
                }
                else
                {
                    _isInitialized = false;
                    //report?
                }
            }
            else
            {
                _isInitialized = false;
                //report?
            }


            _isInitialized = true;
        }
        else
        {
            _isInitialized = false;
        }
        
        return _isInitialized;
    }

    /// <summary>
    /// Extract the file from the XPages URL to a given file path
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public bool ExtractFile(String filePath)
    {
        Connector.ResetReturn();

        if (_isInitialized)
        {
            if (_Document.Database.Session.Connection.Request.ExecuteGetFileRequest(_URL, filePath))
            {
                Connector.hasError = false;
                return true;
            }
        }
        return false;
    }

    #endregion

    #region Methods



    #endregion

}