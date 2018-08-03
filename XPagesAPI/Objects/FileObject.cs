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
public class FileObject {

    #region Variables

    private string strSize = "";
    private string strDateCreated = "";
    private string strDateModified = "";

    #endregion Variables

    #region Properties

    /// <summary>
    /// Reference to this files document object
    /// </summary>
    public DocumentObject Document { get; protected internal set; }

    /// <summary>
    /// The file name of the domino attachment
    /// </summary>
    public string Name { get; protected internal set; } = "";

    /// <summary>
    /// URL of the domino attachment
    /// </summary>
    public string URL { get; protected internal set; } = "";

    /// <summary>
    /// Size of the domino attachment in bytes
    /// </summary>
    public long Size { get; protected internal set; }

    /// <summary>
    /// JPI Soft Class of the domino attachment (Acad,ustn,office)
    /// </summary>
    public string SoftClass { get; protected internal set; } = "";

    /// <summary>
    /// JPI associated application of the domino attachment
    /// </summary>
    public string Application { get; protected internal set; } = "";

    /// <summary>
    /// Creator of the domino attachment
    /// </summary>
    public string Creator { get; protected internal set; } = "";

    /// <summary>
    /// Creation date of the domino attachment
    /// </summary>
    public DateTime DateCreated { get; protected internal set; }

    /// <summary>
    /// Modified date of the domino attachment
    /// </summary>
    public DateTime DateModified { get; protected internal set; }

    /// <summary>
    /// FieldName (Rich-Text item) associated to the domino attachment if available
    /// </summary>
    public string FieldName { get; protected internal set; } = "";

    /// <summary>
    /// The file extension of the domino attachment
    /// </summary>
    public string Extension { get; protected internal set; } = "";

    /// <summary>
    /// Additional JPI information of the domino attachment
    /// </summary>
    public string Other { get; protected internal set; } = "";

    /// <summary>
    /// Indicates that the file isInitialized
    /// </summary>
    public bool IsInitialized { get; protected internal set; } = false;

    #endregion Properties

    #region Constructor

    /// <summary>
    /// FileObject Constructor
    /// </summary>
    internal FileObject(DocumentObject docObj) {
        Document = docObj;
    }

    /// <summary>
    /// Initialize the file object, parses the initialization string into their respective properties
    /// </summary>
    /// <param name="initString">Initialization string</param>
    /// <returns></returns>
    internal bool Initialize(string initString) {
        
        //split the string into the properties here
        if (!String.IsNullOrEmpty(initString)) {
            // fObj.Application + "§" + fObj.Creator + "§" + fObj.DateCreated + "§" + fObj.DateModfied + "$" + fObj.FieldName + "§" + fObj.FileExtension + "§" +
            //fObj.FileName + "$" + fObj.FileSize + "$" + fObj.LinkToFile + "$" + fObj.Other + "$" + fObj.SoftClass);
            if (initString.Contains("§")) {
                String[] ar = initString.Split(new[] { "§" }, StringSplitOptions.None);
                if (ar != null && ar.Length > 0 && ar.Length == 11) {
                    Application = ar[0];
                    Creator = ar[1];
                    strDateCreated = ar[2];
                    if (!string.IsNullOrEmpty(strDateCreated) && !strDateCreated.Equals(".")) {
                        try {
                            //convert to real datetime
                            DateCreated = DateTime.Parse(strDateCreated);
                        } catch (Exception) {
                            //do nothing or report?
                        }
                    }

                    strDateModified = ar[3];
                    if (!string.IsNullOrEmpty(strDateModified) && !strDateModified.Equals(".")) {
                        try {
                            //convert to real datetime
                            DateModified = DateTime.Parse(strDateModified);
                        } catch (Exception) {
                            //do nothing or report?
                        }
                    }

                    FieldName = ar[4];
                    Extension = ar[5];
                    Name = ar[6];
                    strSize = ar[7];
                    if (!string.IsNullOrEmpty(strSize)) {
                        try {
                            Size = long.Parse(strSize);
                            
                        } catch (Exception) {
                            // throw;
                        }
                    }
                    URL = ar[8];
                    Other = ar[9];
                    SoftClass = ar[10];

                    //validating can be done here if not all values are retrieved - atm see what we can get without failing

                    IsInitialized = true;
                } else {
                    IsInitialized = false;
                    //report?
                }
            } else {
                IsInitialized = false;
                //report?
            }

            IsInitialized = true;
        } else {
            IsInitialized = false;
        }

        return IsInitialized;
    }

    /// <summary>
    /// Extract the file from the XPages URL to a given file path
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public bool ExtractFile(String filePath) {
        Connector.ResetReturn();

        if (IsInitialized) {
            if (Document.Database.Session.Connection.Request.ExecuteGetFileRequest(URL, filePath)) {
                Connector.HasError = false;
                return true;
            }
        }
        return false;
    }

    #endregion Constructor
}