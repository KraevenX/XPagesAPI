using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

internal class Requestor {

    #region Variables

    private Connector Connection;
    protected internal bool isInitialized = false;
    private CookieContainer AuthenticationCookie;

    #endregion Variables

    #region Constructor

    public Requestor(Connector Connector) {
        Connection = Connector;
    }

    #endregion Constructor

    #region Private Methods

    private bool AddIdentityHeader(ref HttpWebRequest request) {
        try {
            if (request != null) {
                string XPIdentity = "JPI$XP@ges!C0nn3nt0r|Id3nTity@Request";
                if (!string.IsNullOrEmpty(Connection.XPIdentity)) {
                    XPIdentity = Connection.XPIdentity;
                }
                //  Const XPIdentity As String = "JPI$XP@ges!C0nn3nt0r|Id3nTity@Request"

                Encryptor EncodedEncryptedContent = new Encryptor(XPIdentity, true, ref Connection);
                if (EncodedEncryptedContent.Initialize()) {
                    request.Headers.Add("Identity", EncodedEncryptedContent.EncodedContent);
                    return true;
                }
            } else {
                //request is nothing ' error added in calling function
            }
            return false;
        } catch (Exception ex) {
            Connector.ReturnMessages.Add("Unable to add Identity Header to XPages Request");
            Connector.ReturnMessages.Add(Common.GetErrorInfo(ex));
            Connector.HasError = true; //throws exception

            return false;
        }
    }

    #endregion Private Methods

    #region Protected Internal Methods

    protected internal bool Initialize() {
        HttpWebRequest request = null;

        // use the XPagesAPI ReturnMessages & hasError
        try {
            //ServicePointManager.Expect100Continue = false;
            ServicePointManager.Expect100Continue = true;
            //  ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.DefaultConnectionLimit = 9999;

            // Cookie container to store authentication cookie
            CookieContainer cookies = new CookieContainer();

            //create an HTTP request
            //Login - check for "/names.nsf?Login"
            string serverURL = "";
            serverURL = Connection.ServerURL;
            if (!serverURL.Contains("/names.nsf?Login")) {
                serverURL = serverURL + "/names.nsf?Login";
            }

            request = (HttpWebRequest)WebRequest.Create(serverURL);

            // Prepare HTTP request
            request.Method = WebRequestMethods.Http.Post;
            // "POST"
            request.AllowAutoRedirect = false;
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = cookies;
            //  request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36"
            // Prepare POST body
            string post = "Username=" + Uri.EscapeDataString(Connection.UserName) + "&Password=" + Uri.EscapeDataString(Connection.Password);
            byte[] bytes = Encoding.ASCII.GetBytes(post);

            // Write data to request
            request.ContentLength = bytes.Length;
            Stream streamOut = request.GetRequestStream();
            streamOut.Write(bytes, 0, bytes.Length);
            streamOut.Close();

            //' Get response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Check if we are authenticated properly
            if ((response.StatusCode == HttpStatusCode.OK) || (response.StatusCode == HttpStatusCode.Found) || (response.StatusCode == HttpStatusCode.Redirect) || (response.StatusCode == HttpStatusCode.Moved) || (response.StatusCode == HttpStatusCode.MovedPermanently)) {
                foreach (Cookie cooky in response.Cookies) {
                    cookies.Add(cooky);
                }

                this.AuthenticationCookie = cookies;

                if (response.Cookies != null && response.Cookies.Count > 0) {
                    Connector.ReturnMessages.Add(Connection.UserName + " Successfully Authenticated On Domino Server " + Connection.ServerURL);
                    Connector.HasError = false;
                } else {
                    Connector.ReturnMessages.Add(Connection.UserName + " was unable to authenticate to the domino server " + Connection.ServerURL);
                    Connector.HasError = true;
                    //thows exception
                }
            } else {
                Connector.ReturnMessages.Add(Connection.UserName + " was unable to authenticate to the domino server " + Connection.ServerURL);
                Connector.HasError = true;
                //thows exception
            }
            isInitialized = true;
            return true;
        } catch (Exception ex) {
            Connector.ReturnMessages.Add(Connection.UserName + " was unable to authenticate to the domino server " + Connection.ServerURL);
            Connector.ReturnMessages.Add(Common.GetErrorInfo(ex));
            Connector.HasError = true;
            //throws exception
            isInitialized = false;
            return false;
        }
    }

    protected internal bool ExecuteSessionRequest(string WebServiceURL) {
        HttpWebRequest request = null;

        try {
            if (!isInitialized) {
                Connector.ReturnMessages.Add("Unable to execute the Session Request - Not Initialized/Connected To Server");
                Connector.HasError = true;
                return false;
            }

            //ServicePointManager.Expect100Continue = false;

            //create an HTTP request
            // WebURL use this without ?CheckOut or ?CheckIn

            request = (HttpWebRequest)WebRequest.Create(WebServiceURL + "?API_Session");
            // ?CheckOut

            request.CookieContainer = this.AuthenticationCookie;
            //  request.KeepAlive = True
            request.AllowAutoRedirect = true;

            request.Method = WebRequestMethods.Http.Post;           //"POST"
            request.ContentType = "application/jpi; charset=utf-8";         //application/json
                                                                            // Set the request stream
            string result;

            //add a identifier in the header to be use in JPI Service to check if the request is coming from a valid source aka JPI XPages Connector
            if (!AddIdentityHeader(ref request)) {
                Connector.ReturnMessages.Add("Unable to Execute Session Request : Identity Header could not be added!");
                Connector.HasError = true;
                return false;
            }

            //test with encryption
            StringBuilder sb = new StringBuilder();

            // User Name
            sb.AppendLine("UserName : " + Connection.UserName);
            sb.AppendLine("Connecting via XPagesAPI");

            byte[] b = Encoding.Default.GetBytes(sb.ToString().Replace(Environment.NewLine, " | "));
            string myString = Encoding.UTF8.GetString(b);
            Encryptor EncodedEncryptedContent = new Encryptor(myString, true, ref Connection);
            if (EncodedEncryptedContent.Initialize()) {
                Stream requestStream = null;
                try {
                    requestStream = request.GetRequestStream();
                    using (StreamWriter writer = new StreamWriter(requestStream, Encoding.UTF8)) {
                        writer.Write(EncodedEncryptedContent.EncodedContent);
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the Session Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (requestStream != null) {
                    //    requestStream.Dispose();
                    //}
                }
            } else {
                Connector.ReturnMessages.Add("Executing Session Request - Unable to encode/encrypt the content of the request!");
                Connector.HasError = true; //throws exception
                return false;
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //can give webexception error 500

            if ((response.StatusCode == HttpStatusCode.OK) || (response.StatusCode == HttpStatusCode.Found) || (response.StatusCode == HttpStatusCode.Redirect) || (response.StatusCode == HttpStatusCode.Moved) || (response.StatusCode == HttpStatusCode.MovedPermanently)) {
                Stream responseStream = null;
                try {
                    responseStream = request.GetResponse().GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream, true)) {
                        result = reader.ReadToEnd();
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the Session Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (responseStream != null) {
                    //    responseStream.Dispose();
                    //}
                }

                if (result != null) {
                    //check if the page returned is not just the login page - we need to get encoded & encrypted content here - check if start with <!
                    if (result.StartsWith("<!")) {
                        //possible wrong password !
                        Connector.ReturnMessages.Add("Executing Session Request -  Unable to get the session - Authentication/Password Issue");
                        Connector.HasError = true;
                        //throws exception
                        return false;
                    }

                    Encryptor DecodeDecryptedContent = new Encryptor(result, false, ref Connection);
                    if (DecodeDecryptedContent.Initialize()) {
                        ArrayList arList = new ArrayList();

                        string[] ar = null;
                        bool isError = false;

                        string message = "";
                        string details = "";

                        ar = DecodeDecryptedContent.DecodedContent.Split(new[] { " | " }, StringSplitOptions.None);
                        arList.AddRange(ar);
                        foreach (string str in arList) {
                            //YES/NO
                            if (str.StartsWith("Error: ")) {
                                if (str.Replace("Error: ", "").Equals("Y", StringComparison.OrdinalIgnoreCase)) {
                                    isError = true;
                                }
                            }
                            if (str.StartsWith("Message: ")) {
                                message = str.Replace("Message: ", "");
                            }
                            if (str.StartsWith("Details: ")) {
                                details = str.Replace("Details: ", "");
                            }
                        }
                        if (isError) {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = true;
                            //throws exception
                            return false;
                        } else {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = false;
                            //SUCCESS
                        }
                    } else {
                        Connector.ReturnMessages.Add("Executing Session Request - Unable to decode/decrypt the content of the response!");
                        Connector.HasError = true;
                        //throws exception
                        return false;
                    }
                } else {
                    //no response from server
                    Connector.ReturnMessages.Add("Executing Session Request -  Unable to get response from XPages!");
                    Connector.HasError = true;
                    //throws exception
                    return false;
                }
                return true;
            } else {
                Connector.ReturnMessages.Add("Executing Session Request -  Unable to get the session - Authentication Issue");
                Connector.HasError = true;
                //throws exception
                return false;
            }
        } catch (Exception ex) {
            if (ex.GetType() == typeof(WebException)) {
                WebException webex;
                webex = (WebException)ex;
                if (webex.Response != null) {
                    Stream stream = webex.Response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        string errorResp = reader.ReadToEnd();
                        // errorResp = errorResp.Replace("{", "").Replace("}", "").Replace("\\r\\n\\", Environment.NewLine).Replace(",", Environment.NewLine).Replace(((char)34).ToString(), "").Replace("\\r\\n", Environment.NewLine);
                        errorResp = errorResp.Replace("{", "").Replace("}", "").Replace("\\\\r\\\\n\\\\", Environment.NewLine).Replace(",", Environment.NewLine).Replace(((char)34).ToString(), "").Replace("\\\\r\\\\n", Environment.NewLine);
                        Connector.ReturnMessages.Add("Unable to Execute the Session Request : Invalid request or not authenticated : " + Environment.NewLine + "Web Request Error Response : " + errorResp);
                        Connector.HasError = true;
                        //throws exception
                        return false;
                    }
                } else {
                    Connector.ReturnMessages.Add("Unable to Execute the Session Request : " + Common.GetErrorInfo(ex));
                    Connector.HasError = true;
                    //throws exception
                    return false;
                }
            } else {
                Connector.ReturnMessages.Add("Unable to Execute the Session Request : " + Common.GetErrorInfo(ex));
                Connector.HasError = true;
                //throws exception
                return false;
            }
        }
    }

    protected internal bool ExecuteDatabaseRequest(string WebServiceURL, string ServerName, string dbFilePath, string dbReplicationId, DatabaseObject dbObj) {
        HttpWebRequest request = null;

        try {
            if (!isInitialized) {
                Connector.ReturnMessages.Add("Unable to execute the Database Request - Not Initialized/Connected To Server");
                Connector.HasError = true;
                return false;
            }

            //  ServicePointManager.Expect100Continue = false;

            //create an HTTP request

            request = (HttpWebRequest)WebRequest.Create(WebServiceURL + "?API_Database");

            request.CookieContainer = this.AuthenticationCookie;
            //  request.KeepAlive = True
            request.AllowAutoRedirect = true;

            request.Method = WebRequestMethods.Http.Post;   //"POST"
            request.ContentType = "application/jpi; charset=utf-8"; //application/json
                                                                    // Set the request stream
            string result;

            //add a identifier in the header to be use in JPI Service to check if the request is coming from a valid source aka JPI XPages Connector
            if (!AddIdentityHeader(ref request)) {
                Connector.ReturnMessages.Add("Unable to Execute Database Request : Identity Header could not be added!");
                Connector.HasError = true;  //throws error
                return false;
            }

            //test with encryption
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("UserName: " + Connection.UserName);
            sb.AppendLine("ServerName: " + ServerName);
            sb.AppendLine("FilePath: " + dbFilePath);
            sb.AppendLine("ReplicationID: " + dbReplicationId);

            sb.AppendLine("Connecting via XPagesAPI");

            byte[] b = Encoding.Default.GetBytes(sb.ToString().Replace(Environment.NewLine, " | "));
            string myString = Encoding.UTF8.GetString(b);
            Encryptor EncodedEncryptedContent = new Encryptor(myString, true, ref Connection);
            if (EncodedEncryptedContent.Initialize()) {
                Stream requestStream = null;
                try {
                    requestStream = request.GetRequestStream();
                    using (StreamWriter writer = new StreamWriter(requestStream, Encoding.UTF8)) {
                        writer.Write(EncodedEncryptedContent.EncodedContent);
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the Database Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (requestStream != null) {
                    //    requestStream.Dispose();
                    //}
                }
            } else {
                Connector.ReturnMessages.Add("Executing Database Request - Unable to encode/encrypt the content of the request!");
                Connector.HasError = true; //throws exception
                return false;
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //can give webexception error 500

            if ((response.StatusCode == HttpStatusCode.OK) || (response.StatusCode == HttpStatusCode.Found) || (response.StatusCode == HttpStatusCode.Redirect) || (response.StatusCode == HttpStatusCode.Moved) || (response.StatusCode == HttpStatusCode.MovedPermanently)) {
                Stream responseStream = null;
                try {
                    responseStream = request.GetResponse().GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream, true)) {
                        result = reader.ReadToEnd();
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the Database Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (responseStream != null) {
                    //    responseStream.Dispose();
                    //}
                }

                if (result != null) {
                    Encryptor DecodeDecryptedContent = new Encryptor(result, false, ref Connection);
                    if (DecodeDecryptedContent.Initialize()) {
                        ArrayList arList = new ArrayList();

                        string[] ar = null;
                        bool isError = false;

                        string message = "";
                        string details = "";
                        string value = "";

                        ar = DecodeDecryptedContent.DecodedContent.Split(new[] { " | " }, StringSplitOptions.None);
                        arList.AddRange(ar);
                        foreach (string str in arList) {
                            //YES/NO
                            if (str.StartsWith("Error: ")) {
                                if (str.Replace("Error: ", "").Equals("Y", StringComparison.OrdinalIgnoreCase)) {
                                    isError = true;
                                }
                            }
                            if (str.StartsWith("Message: ")) {
                                message = str.Replace("Message: ", "");
                            }
                            if (str.StartsWith("Details: ")) {
                                details = str.Replace("Details: ", "");
                            }
                            if (str.StartsWith("ServerName: ")) {
                                value = str.Replace("ServerName: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    dbObj.ServerName = value;
                                }
                            }
                            if (str.StartsWith("FilePath: ")) {
                                value = str.Replace("FilePath: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    dbObj.FilePath = value;
                                }
                            }
                            if (str.StartsWith("ReplicationID: ")) {
                                value = str.Replace("ReplicationID: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    dbObj.ReplicationID = value;
                                }
                            }

                            if (str.StartsWith("Title: ")) {
                                value = str.Replace("Title: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    dbObj.Title = value;
                                }
                            }

                            if (str.StartsWith("Template: ")) {
                                value = str.Replace("Template: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    dbObj.Template = value;
                                }
                            }

                            if (str.StartsWith("Size: ")) {
                                value = str.Replace("Size: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    dbObj.Size = value;
                                }
                            }

                            if (str.StartsWith("URL: ")) {
                                value = str.Replace("URL: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    dbObj.Url = value;
                                }
                            }
                        }

                        if (isError) {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = true; //throws exception
                            return false;
                        } else {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = false; //SUCCESS
                        }
                    } else {
                        Connector.ReturnMessages.Add("Executing Database Request - Unable to decode/decrypt the content of the response!");
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    //no response from server
                    Connector.ReturnMessages.Add("Executing Database Request -  Unable to get response from XPages!");
                    Connector.HasError = true;  //throws exception
                    return false;
                }
                return true;
            } else {
                Connector.ReturnMessages.Add("Executing Database Request -  Unable to get the Database - Authentication Issue");
                Connector.HasError = true;  //throws exception
                return false;
            }
        } catch (Exception ex) {
            if (ex.GetType() == typeof(WebException)) {
                WebException webex;
                webex = (WebException)ex;
                if (webex.Response != null) {
                    Stream stream = webex.Response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        string errorResp = reader.ReadToEnd();
                        errorResp = errorResp.Replace("{", "").Replace("}", "").Replace("\\\\r\\\\n\\\\", Environment.NewLine).Replace(",", Environment.NewLine).Replace(((char)34).ToString(), "").Replace("\\\\r\\\\n", Environment.NewLine);
                        Connector.ReturnMessages.Add("Unable to Execute the Database Request : Invalid request or not authenticated : " + Environment.NewLine + "Web Request Error Response : " + errorResp);
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    Connector.ReturnMessages.Add("Unable to Execute the Database Request : " + Common.GetErrorInfo(ex));
                    Connector.HasError = true;  //throws exception
                    return false;
                }
            } else {
                Connector.ReturnMessages.Add("Unable to Execute the Database Request : " + Common.GetErrorInfo(ex));
                Connector.HasError = true;  //throws exception
                return false;
            }
        }
    }

    protected internal bool ExecuteAllDatabasesRequest(string WebServiceURL, string ServerName, SessionObject sObj) {
        HttpWebRequest request = null;

        try {
            if (!isInitialized) {
                Connector.ReturnMessages.Add("Unable to execute the Database Request - Not Initialized/Connected To Server");
                Connector.HasError = true;
                return false;
            }

            //  ServicePointManager.Expect100Continue = false;

            //create an HTTP request

            request = (HttpWebRequest)WebRequest.Create(WebServiceURL + "?API_AllDatabases");

            request.CookieContainer = this.AuthenticationCookie;
            //  request.KeepAlive = True
            request.AllowAutoRedirect = true;

            request.Method = WebRequestMethods.Http.Post;   //"POST"
            request.ContentType = "application/jpi; charset=utf-8"; //application/json
                                                                    // Set the request stream
            string result;

            //add a identifier in the header to be use in JPI Service to check if the request is coming from a valid source aka JPI XPages Connector
            if (!AddIdentityHeader(ref request)) {
                Connector.ReturnMessages.Add("Unable to Execute All Databases Request : Identity Header could not be added!");
                Connector.HasError = true;  //throws error
                return false;
            }

            //test with encryption
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("UserName: " + Connection.UserName);
            sb.AppendLine("ServerName: " + ServerName);

            sb.AppendLine("Connecting via XPagesAPI");

            byte[] b = Encoding.Default.GetBytes(sb.ToString().Replace(Environment.NewLine, " | "));
            string myString = Encoding.UTF8.GetString(b);
            Encryptor EncodedEncryptedContent = new Encryptor(myString, true, ref Connection);
            if (EncodedEncryptedContent.Initialize()) {
                Stream requestStream = null;
                try {
                    requestStream = request.GetRequestStream();
                    using (StreamWriter writer = new StreamWriter(requestStream, Encoding.UTF8)) {
                        writer.Write(EncodedEncryptedContent.EncodedContent);
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the All Databases Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (requestStream != null) {
                    //    requestStream.Dispose();
                    //}
                }
            } else {
                Connector.ReturnMessages.Add("Executing All Databases Request - Unable to encode/encrypt the content of the request!");
                Connector.HasError = true; //throws exception
                return false;
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //can give webexception error 500

            if ((response.StatusCode == HttpStatusCode.OK) || (response.StatusCode == HttpStatusCode.Found) || (response.StatusCode == HttpStatusCode.Redirect) || (response.StatusCode == HttpStatusCode.Moved) || (response.StatusCode == HttpStatusCode.MovedPermanently)) {
                Stream responseStream = null;
                try {
                    responseStream = request.GetResponse().GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream, true)) {
                        result = reader.ReadToEnd();
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the All Databases Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (responseStream != null) {
                    //    responseStream.Dispose();
                    //}
                }

                if (result != null) {
                    Encryptor DecodeDecryptedContent = new Encryptor(result, false, ref Connection);
                    if (DecodeDecryptedContent.Initialize()) {
                        ArrayList arList = new ArrayList();

                        string[] ar = null;
                        bool isError = false;

                        string message = "";
                        string details = "";
                        string value = "";

                        ar = DecodeDecryptedContent.DecodedContent.Split(new[] { " | " }, StringSplitOptions.None);
                        arList.AddRange(ar);
                        DatabaseObject dbObj = null;
                        foreach (string str in arList) {
                            //YES/NO
                            if (str.StartsWith("Error: ")) {
                                if (str.Replace("Error: ", "").Equals("Y", StringComparison.OrdinalIgnoreCase)) {
                                    isError = true;
                                }
                            }
                            if (str.StartsWith("Message: ")) {
                                message = str.Replace("Message: ", "");
                            }
                            if (str.StartsWith("Details: ")) {
                                details = str.Replace("Details: ", "");
                            }

                            if (str.StartsWith("DatabaseObject: ")) {
                                value = str.Replace("DatabaseObject: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    //here we have a line with all database properties separated by §
                                    // FilePath§ServerName§ReplicationID§Title§Template§Size§URL
                                    dbObj = new DatabaseObject(sObj, value);
                                    if (dbObj.IsInitialized) {
                                        if (sObj.Databases == null) {
                                            sObj.Databases = new SortedDictionary<string, DatabaseObject>();
                                        }
                                        if (sObj.Databases.ContainsKey(dbObj.FilePath)) {
                                            sObj.Databases.Remove(dbObj.FilePath);
                                        }
                                        sObj.Databases.Add(dbObj.FilePath, dbObj);
                                    } else {
                                        // Connector error response updated in constructor method - can trigger exception
                                        Connector.ReturnMessages.Add("Error retrieving database object : " + value);
                                    }
                                }
                            }
                        }
                        if (isError) {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = true; //throws exception
                            return false;
                        } else {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = false; //SUCCESS
                        }
                    } else {
                        Connector.ReturnMessages.Add("Executing All Databases Request - Unable to decode/decrypt the content of the response!");
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    //no response from server
                    Connector.ReturnMessages.Add("Executing All Databases Request -  Unable to get response from XPages!");
                    Connector.HasError = true;  //throws exception
                    return false;
                }
                return true;
            } else {
                Connector.ReturnMessages.Add("Executing All Databases Request -  Unable to get the Database - Authentication Issue");
                Connector.HasError = true;  //throws exception
                return false;
            }
        } catch (Exception ex) {
            if (ex.GetType() == typeof(WebException)) {
                WebException webex;
                webex = (WebException)ex;
                if (webex.Response != null) {
                    Stream stream = webex.Response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        string errorResp = reader.ReadToEnd();
                        errorResp = errorResp.Replace("{", "").Replace("}", "").Replace("\\\\r\\\\n\\\\", Environment.NewLine).Replace(",", Environment.NewLine).Replace(((char)34).ToString(), "").Replace("\\\\r\\\\n", Environment.NewLine);
                        Connector.ReturnMessages.Add("Unable to Execute the All Databases Request : Invalid request or not authenticated : " + Environment.NewLine + "Web Request Error Response : " + errorResp);
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    Connector.ReturnMessages.Add("Unable to Execute the All Databases Request : " + Common.GetErrorInfo(ex));
                    Connector.HasError = true;  //throws exception
                    return false;
                }
            } else {
                Connector.ReturnMessages.Add("Unable to Execute the All Databases Request : " + Common.GetErrorInfo(ex));
                Connector.HasError = true;  //throws exception
                return false;
            }
        }
    }

    protected internal bool ExecuteDocumentRequest(string WebServiceURL, string Unid, string searchField, string searchValue, string formula, DatabaseObject dbObj, DocumentObject docObj) {
        HttpWebRequest request = null;

        try {
            if (!isInitialized) {
                Connector.ReturnMessages.Add("Unable to execute the Document Request - Not Initialized/Connected To Server");
                Connector.HasError = true;
                return false;
            }

            // ServicePointManager.Expect100Continue = false;

            //create an HTTP request

            request = (HttpWebRequest)WebRequest.Create(WebServiceURL + "?API_Document");

            request.CookieContainer = this.AuthenticationCookie;
            //  request.KeepAlive = True
            request.AllowAutoRedirect = true;

            request.Method = WebRequestMethods.Http.Post;   //"POST"
            request.ContentType = "application/jpi; charset=utf-8"; //application/json
                                                                    // Set the request stream
            string result;

            //add a identifier in the header to be use in JPI Service to check if the request is coming from a valid source aka JPI XPages Connector
            if (!AddIdentityHeader(ref request)) {
                Connector.ReturnMessages.Add("Unable to Execute Document Request : Identity Header could not be added!");
                Connector.HasError = true;  //throws error
                return false;
            }

            //test with encryption
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("UserName: " + Connection.UserName);
            sb.AppendLine("ServerName: " + docObj.Database.ServerName);
            sb.AppendLine("FilePath: " + docObj.Database.FilePath);
            sb.AppendLine("ReplicationID: " + docObj.Database.ReplicationID);
            sb.AppendLine("UniversalID: " + Unid);
            sb.AppendLine("SearchField: " + searchField);
            sb.AppendLine("SearchValue: " + searchValue);
            sb.AppendLine("Formula: " + formula);
            sb.AppendLine("Connecting via XPagesAPI");

            byte[] b = Encoding.Default.GetBytes(sb.ToString().Replace(Environment.NewLine, " | "));
            string myString = Encoding.UTF8.GetString(b);
            Encryptor EncodedEncryptedContent = new Encryptor(myString, true, ref Connection);
            if (EncodedEncryptedContent.Initialize()) {
                Stream requestStream = null;
                try {
                    requestStream = request.GetRequestStream();
                    using (StreamWriter writer = new StreamWriter(requestStream, Encoding.UTF8)) {
                        writer.Write(EncodedEncryptedContent.EncodedContent);
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (requestStream != null) {
                    //    requestStream.Dispose();
                    //}
                }
            } else {
                Connector.ReturnMessages.Add("Executing Document Request - Unable to encode/encrypt the content of the request!");
                Connector.HasError = true; //throws exception
                return false;
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //can give webexception error 500

            if ((response.StatusCode == HttpStatusCode.OK) || (response.StatusCode == HttpStatusCode.Found) || (response.StatusCode == HttpStatusCode.Redirect) || (response.StatusCode == HttpStatusCode.Moved) || (response.StatusCode == HttpStatusCode.MovedPermanently)) {
                Stream responseStream = null;
                try {
                    responseStream = request.GetResponse().GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream, true)) {
                        result = reader.ReadToEnd();
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (responseStream != null) {
                    //    responseStream.Dispose();
                    //}
                }

                if (result != null) {
                    Encryptor DecodeDecryptedContent = new Encryptor(result, false, ref Connection);
                    if (DecodeDecryptedContent.Initialize()) {
                        ArrayList arList = new ArrayList();

                        string[] ar = null;
                        bool isError = false;

                        string message = "";
                        string details = "";
                        string value = "";
                        string unid = "";
                        string noteId = "";
                        string form = "";
                        string size = "";
                        string url = "";
                        string created = "";
                        string modified = "";

                        ar = DecodeDecryptedContent.DecodedContent.Split(new[] { " | " }, StringSplitOptions.None);
                        arList.AddRange(ar);

                        foreach (string str in arList) {
                            //YES/NO
                            if (str.StartsWith("Error: ")) {
                                if (str.Replace("Error: ", "").Equals("Y", StringComparison.OrdinalIgnoreCase)) {
                                    isError = true;
                                }
                            }
                            if (str.StartsWith("Message: ")) {
                                message = str.Replace("Message: ", "");
                            }
                            if (str.StartsWith("Details: ")) {
                                details = str.Replace("Details: ", "");
                            }

                            if (str.StartsWith("NoteID: ")) {
                                value = str.Replace("NoteID: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    noteId = value;
                                }
                            }
                            if (str.StartsWith("Unid: ")) {
                                value = str.Replace("Unid: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    unid = value;
                                }
                            }
                            if (str.StartsWith("Form: ")) {
                                value = str.Replace("Form: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    form = value;
                                }
                            }

                            if (str.StartsWith("Size: ")) {
                                value = str.Replace("Size: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    size = value;
                                }
                            }

                            if (str.StartsWith("URL: ")) {
                                value = str.Replace("URL: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    url = value;
                                }
                            }

                            if (str.StartsWith("Created: ")) {
                                value = str.Replace("Created: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    created = value;
                                }
                            }

                            if (str.StartsWith("Modified: ")) {
                                value = str.Replace("Modified: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    modified = value;
                                }
                            }
                        }

                        if (isError) {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = true; //throws exception
                            return false;
                        } else {
                            // add doc to list
                            if (dbObj.Documents == null) {
                                dbObj.Documents = new Dictionary<string, DocumentObject>();
                            }
                            if (!string.IsNullOrEmpty(unid)) {
                                //docObj = new DocumentObject(dbObj, unid);
                                docObj.UniversalID = unid;
                                docObj.NoteID = noteId;
                                docObj.Size = size;
                                docObj.Url = url;
                                docObj.Form = form;
                                docObj.DateCreated = created;
                                docObj.DateModified = modified;
                                if (dbObj.Documents.ContainsKey(unid)) {
                                    dbObj.Documents.Remove(unid);
                                }
                                dbObj.Documents.Add(unid, docObj);
                                Connector.ReturnMessages.Add(message);
                                Connector.ReturnMessages.Add(details);
                                Connector.HasError = false; //SUCCESS
                            } else {
                                // unid not found??
                                Connector.ReturnMessages.Add(message);
                                Connector.ReturnMessages.Add(details);
                                Connector.ReturnMessages.Add("Universal ID not found!");
                                Connector.HasError = true; // throws exception
                            }
                        }
                    } else {
                        Connector.ReturnMessages.Add("Executing Document Request - Unable to decode/decrypt the content of the response!");
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    //no response from server
                    Connector.ReturnMessages.Add("Executing Document Request -  Unable to get response from XPages!");
                    Connector.HasError = true;  //throws exception
                    return false;
                }
                return true;
            } else {
                Connector.ReturnMessages.Add("Executing Document Request -  Unable to get the Document - Authentication Issue");
                Connector.HasError = true;  //throws exception
                return false;
            }
        } catch (Exception ex) {
            if (ex.GetType() == typeof(WebException)) {
                WebException webex;
                webex = (WebException)ex;
                if (webex.Response != null) {
                    Stream stream = webex.Response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        string errorResp = reader.ReadToEnd();
                        errorResp = errorResp.Replace("{", "").Replace("}", "").Replace("\\\\r\\\\n\\\\", Environment.NewLine).Replace(",", Environment.NewLine).Replace(((char)34).ToString(), "").Replace("\\\\r\\\\n", Environment.NewLine);
                        Connector.ReturnMessages.Add("Unable to Execute the Document Request : Invalid request or not authenticated : " + Environment.NewLine + "Web Request Error Response : " + errorResp);
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    Connector.ReturnMessages.Add("Unable to Execute the Document Request : " + Common.GetErrorInfo(ex));
                    Connector.HasError = true;  //throws exception
                    return false;
                }
            } else {
                Connector.ReturnMessages.Add("Unable to Execute the Document Request : " + Common.GetErrorInfo(ex));
                Connector.HasError = true;  //throws exception
                return false;
            }
        }
    }

    protected internal bool ExecuteDocumentFilesRequest(string WebServiceURL, string Unid, string searchField, string searchValue, string formula, DatabaseObject dbObj, DocumentObject docObj) {
        HttpWebRequest request = null;

        try {
            if (!isInitialized) {
                Connector.ReturnMessages.Add("Unable to execute the DocumentFiles Request - Not Initialized/Connected To Server");
                Connector.HasError = true;
                return false;
            }

            // ServicePointManager.Expect100Continue = false;

            //create an HTTP request

            request = (HttpWebRequest)WebRequest.Create(WebServiceURL + "?API_Files");

            request.CookieContainer = this.AuthenticationCookie;
            //  request.KeepAlive = True
            request.AllowAutoRedirect = true;

            request.Method = WebRequestMethods.Http.Post;   //"POST"
            request.ContentType = "application/jpi; charset=utf-8"; //application/json
                                                                    // Set the request stream
            string result;

            //add a identifier in the header to be use in JPI Service to check if the request is coming from a valid source aka JPI XPages Connector
            if (!AddIdentityHeader(ref request)) {
                Connector.ReturnMessages.Add("Unable to Execute DocumentFiles Request : Identity Header could not be added!");
                Connector.HasError = true;  //throws error
                return false;
            }

            //test with encryption
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("UserName: " + Connection.UserName);
            sb.AppendLine("ServerName: " + docObj.Database.ServerName);
            sb.AppendLine("FilePath: " + docObj.Database.FilePath);
            sb.AppendLine("ReplicationID: " + docObj.Database.ReplicationID);
            sb.AppendLine("UniversalID: " + Unid);
            sb.AppendLine("SearchField: " + searchField);
            sb.AppendLine("SearchValue: " + searchValue);
            sb.AppendLine("Formula: " + formula);
            sb.AppendLine("Connecting via XPagesAPI");

            byte[] b = Encoding.Default.GetBytes(sb.ToString().Replace(Environment.NewLine, " | "));
            string myString = Encoding.UTF8.GetString(b);
            Encryptor EncodedEncryptedContent = new Encryptor(myString, true, ref Connection);
            if (EncodedEncryptedContent.Initialize()) {
                Stream requestStream = null;
                try {
                    requestStream = request.GetRequestStream();
                    using (StreamWriter writer = new StreamWriter(requestStream, Encoding.UTF8)) {
                        writer.Write(EncodedEncryptedContent.EncodedContent);
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the DocumentFiles Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (requestStream != null) {
                    //    requestStream.Dispose();
                    //}
                }
            } else {
                Connector.ReturnMessages.Add("Executing DocumentFiles Request - Unable to encode/encrypt the content of the request!");
                Connector.HasError = true; //throws exception
                return false;
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //can give webexception error 500

            if ((response.StatusCode == HttpStatusCode.OK) || (response.StatusCode == HttpStatusCode.Found) || (response.StatusCode == HttpStatusCode.Redirect) || (response.StatusCode == HttpStatusCode.Moved) || (response.StatusCode == HttpStatusCode.MovedPermanently)) {
                Stream responseStream = null;
                try {
                    responseStream = request.GetResponse().GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream, true)) {
                        result = reader.ReadToEnd();
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the DocumentFiles Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (responseStream != null) {
                    //    responseStream.Dispose();
                    //}
                }

                if (result != null) {
                    Encryptor DecodeDecryptedContent = new Encryptor(result, false, ref Connection);
                    if (DecodeDecryptedContent.Initialize()) {
                        ArrayList arList = new ArrayList();

                        string[] ar = null;
                        bool isError = false;

                        string message = "";
                        string details = "";
                        string value = "";
                        string unid = "";
                        string noteId = "";
                        string form = "";
                        string size = "";
                        string url = "";
                        string created = "";
                        string modified = "";
                        List<string> fList = new List<String>();

                        ar = DecodeDecryptedContent.DecodedContent.Split(new[] { " | " }, StringSplitOptions.None);
                        arList.AddRange(ar);

                        foreach (string str in arList) {
                            //YES/NO
                            if (str.StartsWith("Error: ")) {
                                if (str.Replace("Error: ", "").Equals("Y", StringComparison.OrdinalIgnoreCase)) {
                                    isError = true;
                                }
                            }
                            if (str.StartsWith("Message: ")) {
                                message = str.Replace("Message: ", "");
                            }
                            if (str.StartsWith("Details: ")) {
                                details = str.Replace("Details: ", "");
                            }

                            if (str.StartsWith("NoteID: ")) {
                                value = str.Replace("NoteID: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    noteId = value;
                                }
                            }
                            if (str.StartsWith("Unid: ")) {
                                value = str.Replace("Unid: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    unid = value;
                                }
                            }
                            if (str.StartsWith("Form: ")) {
                                value = str.Replace("Form: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    form = value;
                                }
                            }

                            if (str.StartsWith("Size: ")) {
                                value = str.Replace("Size: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    size = value;
                                }
                            }

                            if (str.StartsWith("URL: ")) {
                                value = str.Replace("URL: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    url = value;
                                }
                            }

                            if (str.StartsWith("Created: ")) {
                                value = str.Replace("Created: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    created = value;
                                }
                            }

                            if (str.StartsWith("Modified: ")) {
                                value = str.Replace("Modified: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    modified = value;
                                }
                            }

                            if (str.StartsWith("FileObject: ")) {
                                value = str.Replace("FileObject: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    // files = value;
                                    fList.Add(value);
                                }
                            }
                        }

                        if (isError) {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = true; //throws exception
                            return false;
                        } else {
                            // add doc to list
                            if (dbObj.Documents == null) {
                                dbObj.Documents = new Dictionary<string, DocumentObject>();
                            }
                            if (!string.IsNullOrEmpty(unid)) {
                                //docObj = new DocumentObject(dbObj, unid);
                                docObj.UniversalID = unid;
                                docObj.NoteID = noteId;
                                docObj.Size = size;
                                docObj.Url = url;
                                docObj.Form = form;
                                docObj.DateCreated = created;
                                docObj.DateModified = modified;
                                //create file objects
                                SortedDictionary<string, FileObject> files = new SortedDictionary<string, FileObject>();
                                FileObject fObj = null;
                                foreach (string str in fList) {
                                    if (!string.IsNullOrEmpty(str) && !str.Contains("<NO_FILES_ATTACHED>")) {
                                        //can be valid file here split on § and count
                                        //fObj.Application + "§" + fObj.Creator + "§" + fObj.DateCreated + "§" + fObj.DateModfied + "$" + fObj.FieldName + "§" + fObj.FileExtension + "§" + fObj.FileName + "$" + fObj.FileSize + "$" + fObj.LinkToFile + "$" + fObj.Other + "$" + fObj.SoftClass);
                                        fObj = new FileObject(docObj);
                                        if (fObj.Initialize(str)) {
                                            files.Add(fObj.Name, fObj);
                                        }
                                    }
                                }
                                docObj.Files = files;
                                if (dbObj.Documents.ContainsKey(unid)) {
                                    dbObj.Documents.Remove(unid);
                                }
                                dbObj.Documents.Add(unid, docObj);
                                Connector.ReturnMessages.Add(message);
                                Connector.ReturnMessages.Add(details);
                                Connector.HasError = false; //SUCCESS
                            } else {
                                // unid not found??
                                Connector.ReturnMessages.Add(message);
                                Connector.ReturnMessages.Add(details);
                                Connector.ReturnMessages.Add("Universal ID not found!");
                                Connector.HasError = true; // throws exception
                            }
                        }
                    } else {
                        Connector.ReturnMessages.Add("Executing DocumentFiles Request - Unable to decode/decrypt the content of the response!");
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    //no response from server
                    Connector.ReturnMessages.Add("Executing DocumentFiles Request -  Unable to get response from XPages!");
                    Connector.HasError = true;  //throws exception
                    return false;
                }
                return true;
            } else {
                Connector.ReturnMessages.Add("Executing DocumentFiles Request -  Unable to get the Document - Authentication Issue");
                Connector.HasError = true;  //throws exception
                return false;
            }
        } catch (Exception ex) {
            if (ex.GetType() == typeof(WebException)) {
                WebException webex;
                webex = (WebException)ex;
                if (webex.Response != null) {
                    Stream stream = webex.Response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        string errorResp = reader.ReadToEnd();
                        errorResp = errorResp.Replace("{", "").Replace("}", "").Replace("\\\\r\\\\n\\\\", Environment.NewLine).Replace(",", Environment.NewLine).Replace(((char)34).ToString(), "").Replace("\\\\r\\\\n", Environment.NewLine);
                        Connector.ReturnMessages.Add("Unable to Execute the DocumentFiles Request : Invalid request or not authenticated : " + Environment.NewLine + "Web Request Error Response : " + errorResp);
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    Connector.ReturnMessages.Add("Unable to Execute the DocumentFiles Request : " + Common.GetErrorInfo(ex));
                    Connector.HasError = true;  //throws exception
                    return false;
                }
            } else {
                Connector.ReturnMessages.Add("Unable to Execute the DocumentFiles Request : " + Common.GetErrorInfo(ex));
                Connector.HasError = true;  //throws exception
                return false;
            }
        }
    }


    protected internal bool ExecuteDocumentFilesFieldsRequest(string WebServiceURL, string Unid, string searchField, string searchValue, string formula, string fields, DatabaseObject dbObj, DocumentObject docObj) {
        HttpWebRequest request = null;

        try {
            if (!isInitialized) {
                Connector.ReturnMessages.Add("Unable to execute the DocumentFilesFields Request - Not Initialized/Connected To Server");
                Connector.HasError = true;
                return false;
            }

            // ServicePointManager.Expect100Continue = false;

            //create an HTTP request

            request = (HttpWebRequest)WebRequest.Create(WebServiceURL + "?API_FieldsFiles");

            request.CookieContainer = this.AuthenticationCookie;
            //  request.KeepAlive = True
            request.AllowAutoRedirect = true;

            request.Method = WebRequestMethods.Http.Post;   //"POST"
            request.ContentType = "application/jpi; charset=utf-8"; //application/json
                                                                    // Set the request stream
            string result;

            //add a identifier in the header to be use in JPI Service to check if the request is coming from a valid source aka JPI XPages Connector
            if (!AddIdentityHeader(ref request)) {
                Connector.ReturnMessages.Add("Unable to Execute DocumentFilesFields Request : Identity Header could not be added!");
                Connector.HasError = true;  //throws error
                return false;
            }

            //test with encryption
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("UserName: " + Connection.UserName);
            sb.AppendLine("ServerName: " + docObj.Database.ServerName);
            sb.AppendLine("FilePath: " + docObj.Database.FilePath);
            sb.AppendLine("ReplicationID: " + docObj.Database.ReplicationID);
            sb.AppendLine("UniversalID: " + Unid);
            sb.AppendLine("SearchField: " + searchField);
            sb.AppendLine("SearchValue: " + searchValue);
            sb.AppendLine("Formula: " + formula);
            sb.AppendLine("Fields: " + fields);
            sb.AppendLine("Connecting via XPagesAPI");

            byte[] b = Encoding.Default.GetBytes(sb.ToString().Replace(Environment.NewLine, " | "));
            string myString = Encoding.UTF8.GetString(b);
            Encryptor EncodedEncryptedContent = new Encryptor(myString, true, ref Connection);
            if (EncodedEncryptedContent.Initialize()) {
                Stream requestStream = null;
                try {
                    requestStream = request.GetRequestStream();
                    using (StreamWriter writer = new StreamWriter(requestStream, Encoding.UTF8)) {
                        writer.Write(EncodedEncryptedContent.EncodedContent);
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the DocumentFilesFields Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (requestStream != null) {
                    //    requestStream.Dispose();
                    //}
                }
            } else {
                Connector.ReturnMessages.Add("Executing DocumentFilesFields Request - Unable to encode/encrypt the content of the request!");
                Connector.HasError = true; //throws exception
                return false;
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //can give webexception error 500

            if ((response.StatusCode == HttpStatusCode.OK) || (response.StatusCode == HttpStatusCode.Found) || (response.StatusCode == HttpStatusCode.Redirect) || (response.StatusCode == HttpStatusCode.Moved) || (response.StatusCode == HttpStatusCode.MovedPermanently)) {
                Stream responseStream = null;
                try {
                    responseStream = request.GetResponse().GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream, true)) {
                        result = reader.ReadToEnd();
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the DocumentFilesFields Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (responseStream != null) {
                    //    responseStream.Dispose();
                    //}
                }

                if (result != null) {
                    Encryptor DecodeDecryptedContent = new Encryptor(result, false, ref Connection);
                    if (DecodeDecryptedContent.Initialize()) {
                        ArrayList arList = new ArrayList();

                        string[] ar = null;
                        bool isError = false;

                        string message = "";
                        string details = "";
                        string value = "";
                        string unid = "";
                        string noteId = "";
                        string form = "";
                        string size = "";
                        string url = "";
                        string created = "";
                        string modified = "";

                        string[] fieldnames = null;
                        string[] fieldvalues = null;
                        string[] fieldtypes = null;

                        string[] sep = { "||" };

                        List<string> fList = new List<String>();

                        ar = DecodeDecryptedContent.DecodedContent.Split(new[] { " | " }, StringSplitOptions.None);
                        arList.AddRange(ar);

                        foreach (string str in arList) {
                            //YES/NO
                            if (str.StartsWith("Error: ")) {
                                if (str.Replace("Error: ", "").Equals("Y", StringComparison.OrdinalIgnoreCase)) {
                                    isError = true;
                                }
                            }
                            if (str.StartsWith("Message: ")) {
                                message = str.Replace("Message: ", "");
                            }
                            if (str.StartsWith("Details: ")) {
                                details = str.Replace("Details: ", "");
                            }

                            if (str.StartsWith("NoteID: ")) {
                                value = str.Replace("NoteID: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    noteId = value;
                                }
                            }
                            if (str.StartsWith("Unid: ")) {
                                value = str.Replace("Unid: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    unid = value;
                                }
                            }
                            if (str.StartsWith("Form: ")) {
                                value = str.Replace("Form: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    form = value;
                                }
                            }

                            if (str.StartsWith("Size: ")) {
                                value = str.Replace("Size: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    size = value;
                                }
                            }

                            if (str.StartsWith("URL: ")) {
                                value = str.Replace("URL: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    url = value;
                                }
                            }

                            if (str.StartsWith("Created: ")) {
                                value = str.Replace("Created: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    created = value;
                                }
                            }

                            if (str.StartsWith("Modified: ")) {
                                value = str.Replace("Modified: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    modified = value;
                                }
                            }

                            if (str.StartsWith("FileObject: ")) {
                                value = str.Replace("FileObject: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    // files = value;
                                    fList.Add(value);
                                }
                            }

                            if (str.StartsWith("FieldNames: ")) {
                                value = str.Replace("FieldNames: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    fieldnames = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("FieldValues: ")) {
                                value = str.Replace("FieldValues: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    fieldvalues = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("FieldTypes: ")) {
                                value = str.Replace("FieldTypes: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    fieldtypes = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                          
                        }

                        if (isError) {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = true; //throws exception
                            return false;
                        } else {
                            // add doc to list
                            if (dbObj.Documents == null) {
                                dbObj.Documents = new Dictionary<string, DocumentObject>();
                            }
                            if (!string.IsNullOrEmpty(unid)) {
                                //docObj = new DocumentObject(dbObj, unid);
                                docObj.UniversalID = unid;
                                docObj.NoteID = noteId;
                                docObj.Size = size;
                                docObj.Url = url;
                                docObj.Form = form;
                                docObj.DateCreated = created;
                                docObj.DateModified = modified;
                                //create file objects
                                SortedDictionary<string, FileObject> files = new SortedDictionary<string, FileObject>();
                                FileObject fObj = null;
                                foreach (string str in fList) {
                                    if (!string.IsNullOrEmpty(str) && !str.Contains("<NO_FILES_ATTACHED>")) {
                                        //can be valid file here split on § and count
                                        //fObj.Application + "§" + fObj.Creator + "§" + fObj.DateCreated + "§" + fObj.DateModfied + "$" + fObj.FieldName + "§" + fObj.FileExtension + "§" + fObj.FileName + "$" + fObj.FileSize + "$" + fObj.LinkToFile + "$" + fObj.Other + "$" + fObj.SoftClass);
                                        fObj = new FileObject(docObj);
                                        if (fObj.Initialize(str)) {
                                            files.Add(fObj.Name, fObj);
                                        }
                                    }
                                }
                                docObj.Files = files;

                                // create fields
                                FieldObject fieldObj = null;
                                if (docObj.Fields == null) {
                                    docObj.Fields = new SortedDictionary<string, FieldObject>();
                                }
                                for (int i = 0; i < fieldnames.Length; i++) {
                                    fieldObj = new FieldObject(fieldnames[i]);
                                    if (fieldvalues.Length - 1 >= i) {
                                        fieldObj.Value = fieldvalues[i];
                                    }
                                    if (fieldtypes.Length - 1 >= i) {
                                        fieldObj.Type = fieldtypes[i];
                                    }

                                    if (docObj.Fields.ContainsKey(fieldObj.Name)) {
                                        docObj.Fields.Remove(fieldObj.Name);
                                    }
                                    docObj.Fields.Add(fieldObj.Name, fieldObj);
                                }

                                if (dbObj.Documents.ContainsKey(unid)) {
                                    dbObj.Documents.Remove(unid);
                                }
                                dbObj.Documents.Add(unid, docObj);

                                Connector.ReturnMessages.Add(message);
                                Connector.ReturnMessages.Add(details);
                                Connector.HasError = false; //SUCCESS
                            } else {
                                // unid not found??
                                Connector.ReturnMessages.Add(message);
                                Connector.ReturnMessages.Add(details);
                                Connector.ReturnMessages.Add("Universal ID not found!");
                                Connector.HasError = true; // throws exception
                            }
                        }
                    } else {
                        Connector.ReturnMessages.Add("Executing DocumentFilesFields Request - Unable to decode/decrypt the content of the response!");
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    //no response from server
                    Connector.ReturnMessages.Add("Executing DocumentFilesFields Request -  Unable to get response from XPages!");
                    Connector.HasError = true;  //throws exception
                    return false;
                }
                return true;
            } else {
                Connector.ReturnMessages.Add("Executing DocumentFilesFields Request -  Unable to get the Document - Authentication Issue");
                Connector.HasError = true;  //throws exception
                return false;
            }
        } catch (Exception ex) {
            if (ex.GetType() == typeof(WebException)) {
                WebException webex;
                webex = (WebException)ex;
                if (webex.Response != null) {
                    Stream stream = webex.Response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        string errorResp = reader.ReadToEnd();
                        errorResp = errorResp.Replace("{", "").Replace("}", "").Replace("\\\\r\\\\n\\\\", Environment.NewLine).Replace(",", Environment.NewLine).Replace(((char)34).ToString(), "").Replace("\\\\r\\\\n", Environment.NewLine);
                        Connector.ReturnMessages.Add("Unable to Execute the DocumentFilesFields Request : Invalid request or not authenticated : " + Environment.NewLine + "Web Request Error Response : " + errorResp);
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    Connector.ReturnMessages.Add("Unable to Execute the DocumentFilesFields Request : " + Common.GetErrorInfo(ex));
                    Connector.HasError = true;  //throws exception
                    return false;
                }
            } else {
                Connector.ReturnMessages.Add("Unable to Execute the DocumentFilesFields Request : " + Common.GetErrorInfo(ex));
                Connector.HasError = true;  //throws exception
                return false;
            }
        }
    }

    protected internal bool ExecuteAllDocumentsRequest(string WebServiceURL, string searchField, string searchValue, string formula, string unids, DatabaseObject dbObj) {
        HttpWebRequest request = null;

        try {
            if (!isInitialized) {
                Connector.ReturnMessages.Add("Unable to execute the All Documents Request - Not Initialized/Connected To Server");
                Connector.HasError = true;
                return false;
            }

            //  ServicePointManager.Expect100Continue = false;

            //create an HTTP request

            request = (HttpWebRequest)WebRequest.Create(WebServiceURL + "?API_AllDocuments");
            //request.Timeout = 300000; // set timeout to 5minutes
            request.Timeout = 1800000; //half an hour!
            request.CookieContainer = this.AuthenticationCookie;
            //  request.KeepAlive = True
            request.AllowAutoRedirect = true;

            request.Method = WebRequestMethods.Http.Post;   //"POST"
            request.ContentType = "application/jpi; charset=utf-8"; //application/json
                                                                    // Set the request stream
            string result;

            //add a identifier in the header to be use in JPI Service to check if the request is coming from a valid source aka JPI XPages Connector
            if (!AddIdentityHeader(ref request)) {
                Connector.ReturnMessages.Add("Unable to Execute All Documents Request : Identity Header could not be added!");
                Connector.HasError = true;  //throws error
                return false;
            }

            //test with encryption
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("UserName: " + Connection.UserName);
            sb.AppendLine("ServerName: " + dbObj.ServerName);
            sb.AppendLine("FilePath: " + dbObj.FilePath);
            sb.AppendLine("ReplicationID: " + dbObj.ReplicationID);
            sb.AppendLine("SearchField: " + searchField);
            sb.AppendLine("SearchValue: " + searchValue);
            sb.AppendLine("Formula: " + formula);
            sb.AppendLine("Unids: " + unids);
            sb.AppendLine("Connecting via XPagesAPI");

            byte[] b = Encoding.Default.GetBytes(sb.ToString().Replace(Environment.NewLine, " | "));
            string myString = Encoding.UTF8.GetString(b);
            Encryptor EncodedEncryptedContent = new Encryptor(myString, true, ref Connection);
            if (EncodedEncryptedContent.Initialize()) {
                Stream requestStream = null;
                try {
                    requestStream = request.GetRequestStream();
                    using (StreamWriter writer = new StreamWriter(requestStream, Encoding.UTF8)) {
                        writer.Write(EncodedEncryptedContent.EncodedContent);
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the All Documents Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (requestStream != null) {
                    //    requestStream.Dispose();
                    //}
                }
            } else {
                Connector.ReturnMessages.Add("Executing All Documents Request - Unable to encode/encrypt the content of the request!");
                Connector.HasError = true; //throws exception
                return false;
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //can give webexception error 500

            if ((response.StatusCode == HttpStatusCode.OK) || (response.StatusCode == HttpStatusCode.Found) || (response.StatusCode == HttpStatusCode.Redirect) || (response.StatusCode == HttpStatusCode.Moved) || (response.StatusCode == HttpStatusCode.MovedPermanently)) {
                Stream responseStream = null;
                try {
                    responseStream = request.GetResponse().GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream, true)) {
                        result = reader.ReadToEnd();
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the All Documents Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (responseStream != null) {
                    //    responseStream.Dispose();
                    //}
                }

                if (result != null) {
                    Encryptor DecodeDecryptedContent = new Encryptor(result, false, ref Connection);
                    if (DecodeDecryptedContent.Initialize()) {
                        ArrayList arList = new ArrayList();

                        string[] ar = null;
                        bool isError = false;

                        string message = "";
                        string details = "";
                        string value = "";
                        string[] sep = { "||" };
                        string[] NoteIds = { };
                        string[] Unids = { };
                        string[] Forms = { };
                        string[] Sizes = { };
                        string[] URLs = { };
                        string[] Created = { };
                        string[] Modified = { };

                        ar = DecodeDecryptedContent.DecodedContent.Split(new[] { " | " }, StringSplitOptions.None);
                        arList.AddRange(ar);
                        foreach (string str in arList) {
                            //YES/NO
                            if (str.StartsWith("Error: ")) {
                                if (str.Replace("Error: ", "").Equals("Y", StringComparison.OrdinalIgnoreCase)) {
                                    isError = true;
                                }
                            }
                            if (str.StartsWith("Message: ")) {
                                message = str.Replace("Message: ", "");
                            }
                            if (str.StartsWith("Details: ")) {
                                details = str.Replace("Details: ", "");
                            }

                            if (str.StartsWith("Unids: ")) {
                                value = str.Replace("Unids: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Unids = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("NoteIDs: ")) {
                                value = str.Replace("NoteIDs: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    NoteIds = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("Forms: ")) {
                                value = str.Replace("Forms: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Forms = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("Sizes: ")) {
                                value = str.Replace("Sizes: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Sizes = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("URLs: ")) {
                                value = str.Replace("URLs: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    URLs = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("Created: ")) {
                                value = str.Replace("Created: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Created = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("Modified: ")) {
                                value = str.Replace("Modified: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Modified = value.Split(sep, StringSplitOptions.None);
                                }
                            }
                        }

                        //loop through unids, notesids, forms, sizes, urls and create document objects
                        // check if all have the same number of items!
                        int count = 0;
                        if (Unids != null && Unids.Length > 0) {
                            count = Unids.Length;
                            if (NoteIds != null && NoteIds.Length > 0) {
                                if (count != NoteIds.Length) {
                                    Connector.ReturnMessages.Add("Executing All Documents Request -  Unable to get the Documents - Unids <> NoteIds");
                                    Connector.HasError = true;  //throws exception
                                    return false;
                                } else {
                                    // form can be empty!!
                                    //if (Forms != null && Forms.Length > 0) {
                                    //    if (count != Forms.Length) {
                                    //        Connector.ReturnMessages.Add("Executing All Documents Request -  Unable to get the Documents - Unids <> Forms");
                                    //        Connector.hasError = true;  //throws exception
                                    //        return false;
                                    //    } else {
                                    if (Sizes != null && Sizes.Length > 0) {
                                        if (count != Sizes.Length) {
                                            Connector.ReturnMessages.Add("Executing All Documents Request -  Unable to get the Documents - Unids <> Sizes");
                                            Connector.HasError = true;  //throws exception
                                            return false;
                                        } else {
                                            if (URLs != null && URLs.Length > 0) {
                                                if (count != URLs.Length) {
                                                    Connector.ReturnMessages.Add("Executing All Documents Request -  Unable to get the Documents - Unids <> URLs");
                                                    Connector.HasError = true;  //throws exception
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                    //   }
                                    // }
                                }
                            }
                            // loop through and create doc obj
                            DocumentObject docObj = null;
                            if (dbObj.Documents == null) {
                                dbObj.Documents = new Dictionary<string, DocumentObject>();
                            }
                            for (int i = 0; i < Unids.Length; i++) {
                                docObj = new DocumentObject(dbObj, Unids[i]);
                                if (NoteIds.Length - 1 >= i) {
                                    docObj.NoteID = NoteIds[i];
                                }
                                if (Forms.Length - 1 >= i) {
                                    docObj.Form = Forms[i];
                                }
                                if (Sizes.Length - 1 >= i) {
                                    docObj.Size = Sizes[i];
                                }
                                if (URLs.Length - 1 >= i) {
                                    docObj.Url = URLs[i];
                                }
                                if (Created.Length - 1 >= i) {
                                    docObj.DateCreated = Created[i];
                                }
                                if (Modified.Length - 1 >= i) {
                                    docObj.DateModified = Modified[i];
                                }
                                if (dbObj.Documents.ContainsKey(Unids[i])) {
                                    dbObj.Documents.Remove(Unids[i]);
                                }
                                docObj.IsInitialized = true;
                                dbObj.Documents.Add(Unids[i], docObj);
                            }
                        }

                        if (isError) {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = true; //throws exception
                            return false;
                        } else {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = false; //SUCCESS
                        }
                    } else {
                        Connector.ReturnMessages.Add("Executing All Documents Request - Unable to decode/decrypt the content of the response!");
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    //no response from server
                    Connector.ReturnMessages.Add("Executing All Documents Request -  Unable to get response from XPages!");
                    Connector.HasError = true;  //throws exception
                    return false;
                }
                return true;
            } else {
                Connector.ReturnMessages.Add("Executing All Documents Request -  Unable to get the Document - Authentication Issue");
                Connector.HasError = true;  //throws exception
                return false;
            }
        } catch (Exception ex) {
            if (ex.GetType() == typeof(WebException)) {
                WebException webex;
                webex = (WebException)ex;
                if (webex.Response != null) {
                    Stream stream = webex.Response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        string errorResp = reader.ReadToEnd();
                        errorResp = errorResp.Replace("{", "").Replace("}", "").Replace("\\\\r\\\\n\\\\", Environment.NewLine).Replace(",", Environment.NewLine).Replace(((char)34).ToString(), "").Replace("\\\\r\\\\n", Environment.NewLine);
                        Connector.ReturnMessages.Add("Unable to Execute the All Documents Request : Invalid request or not authenticated : " + Environment.NewLine + "Web Request Error Response : " + errorResp);
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    Connector.ReturnMessages.Add("Unable to Execute the All Documents Request : " + Common.GetErrorInfo(ex));
                    Connector.HasError = true;  //throws exception
                    return false;
                }
            } else {
                Connector.ReturnMessages.Add("Unable to Execute the All Documents Request : " + Common.GetErrorInfo(ex));
                Connector.HasError = true;  //throws exception
                return false;
            }
        }
    }

    protected internal bool ExecuteAllDocumentsFilesRequest(string WebServiceURL, string searchField, string searchValue, string formula, string unids, DatabaseObject dbObj) {
        HttpWebRequest request = null;

        try {
            if (!isInitialized) {
                Connector.ReturnMessages.Add("Unable to execute the AllDocumentsFiles Request - Not Initialized/Connected To Server");
                Connector.HasError = true;
                return false;
            }

            //    ServicePointManager.Expect100Continue = false;

            //create an HTTP request

            request = (HttpWebRequest)WebRequest.Create(WebServiceURL + "?API_AllFiles");
            //request.Timeout = 300000; // set timeout to 5minutes
            request.Timeout = 1800000; //half an hour!
            request.CookieContainer = this.AuthenticationCookie;
            //  request.KeepAlive = True
            request.AllowAutoRedirect = true;

            request.Method = WebRequestMethods.Http.Post;   //"POST"
            request.ContentType = "application/jpi; charset=utf-8"; //application/json
                                                                    // Set the request stream
            string result;

            //add a identifier in the header to be use in JPI Service to check if the request is coming from a valid source aka JPI XPages Connector
            if (!AddIdentityHeader(ref request)) {
                Connector.ReturnMessages.Add("Unable to Execute AllDocumentsFiles Request : Identity Header could not be added!");
                Connector.HasError = true;  //throws error
                return false;
            }

            //test with encryption
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("UserName: " + Connection.UserName);
            sb.AppendLine("ServerName: " + dbObj.ServerName);
            sb.AppendLine("FilePath: " + dbObj.FilePath);
            sb.AppendLine("ReplicationID: " + dbObj.ReplicationID);
            sb.AppendLine("SearchField: " + searchField);
            sb.AppendLine("SearchValue: " + searchValue);
            sb.AppendLine("Formula: " + formula);
            sb.AppendLine("Unids: " + unids);
            sb.AppendLine("Connecting via XPagesAPI");

            byte[] b = Encoding.Default.GetBytes(sb.ToString().Replace(Environment.NewLine, " | "));
            string myString = Encoding.UTF8.GetString(b);
            Encryptor EncodedEncryptedContent = new Encryptor(myString, true, ref Connection);
            if (EncodedEncryptedContent.Initialize()) {
                Stream requestStream = null;
                try {
                    requestStream = request.GetRequestStream();
                    using (StreamWriter writer = new StreamWriter(requestStream, Encoding.UTF8)) {
                        writer.Write(EncodedEncryptedContent.EncodedContent);
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the AllDocumentsFiles Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (requestStream != null) {
                    //    requestStream.Dispose();
                    //}
                }
            } else {
                Connector.ReturnMessages.Add("Executing AllDocumentsFiles Request - Unable to encode/encrypt the content of the request!");
                Connector.HasError = true; //throws exception
                return false;
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //can give webexception error 500

            if ((response.StatusCode == HttpStatusCode.OK) || (response.StatusCode == HttpStatusCode.Found) || (response.StatusCode == HttpStatusCode.Redirect) || (response.StatusCode == HttpStatusCode.Moved) || (response.StatusCode == HttpStatusCode.MovedPermanently)) {
                Stream responseStream = null;
                try {
                    responseStream = request.GetResponse().GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream, true)) {
                        result = reader.ReadToEnd();
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the AllDocumentsFiles Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (responseStream != null) {
                    //    responseStream.Dispose();
                    //}
                }

                if (result != null) {
                    Encryptor DecodeDecryptedContent = new Encryptor(result, false, ref Connection);
                    if (DecodeDecryptedContent.Initialize()) {
                        ArrayList arList = new ArrayList();

                        string[] ar = null;
                        bool isError = false;

                        string message = "";
                        string details = "";
                        string value = "";
                        string[] sep = { "||" };
                        string[] NoteIds = { };
                        string[] Unids = { };
                        string[] Forms = { };
                        string[] Sizes = { };
                        string[] URLs = { };
                        string[] Created = { };
                        string[] Modified = { };
                        string[] Files = { };

                        ar = DecodeDecryptedContent.DecodedContent.Split(new[] { " | " }, StringSplitOptions.None);
                        arList.AddRange(ar);
                        foreach (string str in arList) {
                            //YES/NO
                            if (str.StartsWith("Error: ")) {
                                if (str.Replace("Error: ", "").Equals("Y", StringComparison.OrdinalIgnoreCase)) {
                                    isError = true;
                                }
                            }
                            if (str.StartsWith("Message: ")) {
                                message = str.Replace("Message: ", "");
                            }
                            if (str.StartsWith("Details: ")) {
                                details = str.Replace("Details: ", "");
                            }

                            if (str.StartsWith("Unids: ")) {
                                value = str.Replace("Unids: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Unids = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("NoteIDs: ")) {
                                value = str.Replace("NoteIDs: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    NoteIds = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("Forms: ")) {
                                value = str.Replace("Forms: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Forms = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("Sizes: ")) {
                                value = str.Replace("Sizes: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Sizes = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("URLs: ")) {
                                value = str.Replace("URLs: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    URLs = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("Created: ")) {
                                value = str.Replace("Created: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Created = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("Modified: ")) {
                                value = str.Replace("Modified: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Modified = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("FileObjects: ")) {
                                value = str.Replace("FileObjects: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Files = value.Split(sep, StringSplitOptions.None);
                                }
                            }
                        }

                        //loop through unids, notesids, forms, sizes, urls and create document objects
                        // check if all have the same number of items!
                        int count = 0;
                        if (Unids != null && Unids.Length > 0) {
                            count = Unids.Length;
                            if (NoteIds != null && NoteIds.Length > 0) {
                                if (count != NoteIds.Length) {
                                    Connector.ReturnMessages.Add("Executing AllDocumentsFiles Request -  Unable to get the Documents - Unids <> NoteIds");
                                    Connector.HasError = true;  //throws exception
                                    return false;
                                } else {
                                    // form can be empty!!
                                    //if (Forms != null && Forms.Length > 0) {
                                    //    if (count != Forms.Length) {
                                    //        Connector.ReturnMessages.Add("Executing All Documents Request -  Unable to get the Documents - Unids <> Forms");
                                    //        Connector.hasError = true;  //throws exception
                                    //        return false;
                                    //    } else {
                                    if (Sizes != null && Sizes.Length > 0) {
                                        if (count != Sizes.Length) {
                                            Connector.ReturnMessages.Add("Executing AllDocumentsFiles Request -  Unable to get the Documents - Unids <> Sizes");
                                            Connector.HasError = true;  //throws exception
                                            return false;
                                        } else {
                                            if (URLs != null && URLs.Length > 0) {
                                                if (count != URLs.Length) {
                                                    Connector.ReturnMessages.Add("Executing AllDocumentsFiles Request -  Unable to get the Documents - Unids <> URLs");
                                                    Connector.HasError = true;  //throws exception
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                    //   }
                                    // }
                                }
                            }
                            // loop through and create doc obj
                            DocumentObject docObj = null;
                            if (dbObj.Documents == null) {
                                dbObj.Documents = new Dictionary<string, DocumentObject>();
                            }
                            for (int i = 0; i < Unids.Length; i++) {
                                docObj = new DocumentObject(dbObj, Unids[i]);
                                if (NoteIds.Length - 1 >= i) {
                                    docObj.NoteID = NoteIds[i];
                                }
                                if (Forms.Length - 1 >= i) {
                                    docObj.Form = Forms[i];
                                }
                                if (Sizes.Length - 1 >= i) {
                                    docObj.Size = Sizes[i];
                                }
                                if (URLs.Length - 1 >= i) {
                                    docObj.Url = URLs[i];
                                }
                                if (Created.Length - 1 >= i) {
                                    docObj.DateCreated = Created[i];
                                }
                                if (Modified.Length - 1 >= i) {
                                    docObj.DateModified = Modified[i];
                                }
                                if (Files.Length - 1 >= i) {
                                    //docObj.DateModified = Files[i];
                                    //get all files into fileobjects here - split on 'FileObject: '
                                    docObj.Files = new SortedDictionary<string, FileObject>();
                                    FileObject fObj = null;
                                    string[] arFiles = Files[i].Split(new[] { "§§FileObject: " }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (string str in arFiles) {
                                        if (!string.IsNullOrEmpty(str) && !str.Contains("<NO_FILES_ATTACHED>")) {
                                            //can be valid file here split on § and count
                                            //fObj.Application + "§" + fObj.Creator + "§" + fObj.DateCreated + "§" + fObj.DateModfied + "$" + fObj.FieldName + "§" + fObj.FileExtension + "§" + fObj.FileName + "§" + fObj.FileSize + "§" + fObj.LinkToFile + "§" + fObj.Other + "§" + fObj.SoftClass);
                                            fObj = new FileObject(docObj);
                                            if (fObj.Initialize(str)) {
                                                docObj.Files.Add(fObj.Name, fObj);
                                            }
                                        }
                                    }
                                }
                                if (dbObj.Documents.ContainsKey(Unids[i])) {
                                    dbObj.Documents.Remove(Unids[i]);
                                }
                                docObj.IsInitialized = true;
                                dbObj.Documents.Add(Unids[i], docObj);
                            }
                        }

                        if (isError) {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = true; //throws exception
                            return false;
                        } else {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = false; //SUCCESS
                        }
                    } else {
                        Connector.ReturnMessages.Add("Executing AllDocumentsFiles Request - Unable to decode/decrypt the content of the response!");
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    //no response from server
                    Connector.ReturnMessages.Add("Executing AllDocumentsFiles Request -  Unable to get response from XPages!");
                    Connector.HasError = true;  //throws exception
                    return false;
                }
                return true;
            } else {
                Connector.ReturnMessages.Add("Executing AllDocumentsFiles Request -  Unable to get the Document - Authentication Issue");
                Connector.HasError = true;  //throws exception
                return false;
            }
        } catch (Exception ex) {
            if (ex.GetType() == typeof(WebException)) {
                WebException webex;
                webex = (WebException)ex;
                if (webex.Response != null) {
                    Stream stream = webex.Response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        string errorResp = reader.ReadToEnd();
                        errorResp = errorResp.Replace("{", "").Replace("}", "").Replace("\\\\r\\\\n\\\\", Environment.NewLine).Replace(",", Environment.NewLine).Replace(((char)34).ToString(), "").Replace("\\\\r\\\\n", Environment.NewLine);
                        Connector.ReturnMessages.Add("Unable to Execute the AllDocumentsFiles Request : Invalid request or not authenticated : " + Environment.NewLine + "Web Request Error Response : " + errorResp);
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    Connector.ReturnMessages.Add("Unable to Execute the AllDocumentsFiles Request : " + Common.GetErrorInfo(ex));
                    Connector.HasError = true;  //throws exception
                    return false;
                }
            } else {
                Connector.ReturnMessages.Add("Unable to Execute the AllDocumentsFiles Request : " + Common.GetErrorInfo(ex));
                Connector.HasError = true;  //throws exception
                return false;
            }
        }
    }

    protected internal bool ExecuteFieldsRequest(string WebServiceURL, DocumentObject docObj, string fields) {
        HttpWebRequest request = null;

        try {
            if (!isInitialized) {
                Connector.ReturnMessages.Add("Unable to execute the Fields Request - Not Initialized/Connected To Server");
                Connector.HasError = true;
                return false;
            }

            // ServicePointManager.Expect100Continue = false;

            //create an HTTP request

            request = (HttpWebRequest)WebRequest.Create(WebServiceURL + "?API_Fields");

            request.CookieContainer = this.AuthenticationCookie;
            //  request.KeepAlive = True
            request.AllowAutoRedirect = true;

            request.Method = WebRequestMethods.Http.Post;   //"POST"
            request.ContentType = "application/jpi; charset=utf-8"; //application/json
                                                                    // Set the request stream
            string result;

            //add a identifier in the header to be use in JPI Service to check if the request is coming from a valid source aka JPI XPages Connector
            if (!AddIdentityHeader(ref request)) {
                Connector.ReturnMessages.Add("Unable to Execute Fields Request : Identity Header could not be added!");
                Connector.HasError = true;  //throws error
                return false;
            }

            //test with encryption
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("UserName: " + Connection.UserName);
            sb.AppendLine("ServerName: " + docObj.Database.ServerName);
            sb.AppendLine("FilePath: " + docObj.Database.FilePath);
            sb.AppendLine("ReplicationID: " + docObj.Database.ReplicationID);
            sb.AppendLine("UniversalID: " + docObj.UniversalID);
            sb.AppendLine("Fields: " + fields);
            sb.AppendLine("Connecting via XPagesAPI");

            byte[] b = Encoding.Default.GetBytes(sb.ToString().Replace(Environment.NewLine, " | "));
            string myString = Encoding.UTF8.GetString(b);
            Encryptor EncodedEncryptedContent = new Encryptor(myString, true, ref Connection);
            if (EncodedEncryptedContent.Initialize()) {
                Stream requestStream = null;
                try {
                    requestStream = request.GetRequestStream();
                    using (StreamWriter writer = new StreamWriter(requestStream, Encoding.UTF8)) {
                        writer.Write(EncodedEncryptedContent.EncodedContent);
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (requestStream != null) {
                    //    requestStream.Dispose();
                    //}
                }
            } else {
                Connector.ReturnMessages.Add("Executing Fields Request - Unable to encode/encrypt the content of the request!");
                Connector.HasError = true; //throws exception
                return false;
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //can give webexception error 500

            if ((response.StatusCode == HttpStatusCode.OK) || (response.StatusCode == HttpStatusCode.Found) || (response.StatusCode == HttpStatusCode.Redirect) || (response.StatusCode == HttpStatusCode.Moved) || (response.StatusCode == HttpStatusCode.MovedPermanently)) {
                Stream responseStream = null;
                try {
                    responseStream = request.GetResponse().GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream, true)) {
                        result = reader.ReadToEnd();
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (responseStream != null) {
                    //    responseStream.Dispose();
                    //}
                }

                if (result != null) {
                    Encryptor DecodeDecryptedContent = new Encryptor(result, false, ref Connection);
                    if (DecodeDecryptedContent.Initialize()) {
                        ArrayList arList = new ArrayList();

                        string[] ar = null;
                        bool isError = false;

                        string message = "";
                        string details = "";
                        string value = "";
                        string[] fieldnames = null;
                        string[] fieldvalues = null;
                        string[] fieldtypes = null;

                        string[] sep = { "||" };

                        ar = DecodeDecryptedContent.DecodedContent.Split(new[] { " | " }, StringSplitOptions.None);
                        arList.AddRange(ar);
                        foreach (string str in arList) {
                            //YES/NO
                            if (str.StartsWith("Error: ")) {
                                if (str.Replace("Error: ", "").Equals("Y", StringComparison.OrdinalIgnoreCase)) {
                                    isError = true;
                                }
                            }
                            if (str.StartsWith("Message: ")) {
                                message = str.Replace("Message: ", "");
                            }
                            if (str.StartsWith("Details: ")) {
                                details = str.Replace("Details: ", "");
                            }

                            if (str.StartsWith("FieldNames: ")) {
                                value = str.Replace("FieldNames: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    fieldnames = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("FieldValues: ")) {
                                value = str.Replace("FieldValues: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    fieldvalues = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("FieldTypes: ")) {
                                value = str.Replace("FieldTypes: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    fieldtypes = value.Split(sep, StringSplitOptions.None);
                                }
                            }
                        }
                        FieldObject fObj = null;
                        if (docObj.Fields == null) {
                            docObj.Fields = new SortedDictionary<string, FieldObject>();
                        }
                        for (int i = 0; i < fieldnames.Length; i++) {
                            fObj = new FieldObject(fieldnames[i]);
                            if (fieldvalues.Length - 1 >= i) {
                                fObj.Value = fieldvalues[i];
                            }
                            if (fieldtypes.Length - 1 >= i) {
                                fObj.Type = fieldtypes[i];
                            }

                            if (docObj.Fields.ContainsKey(fObj.Name)) {
                                docObj.Fields.Remove(fObj.Name);
                            }
                            docObj.Fields.Add(fObj.Name, fObj);
                        }

                        if (isError) {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = true; //throws exception
                            return false;
                        } else {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = false; //SUCCESS
                        }
                    } else {
                        Connector.ReturnMessages.Add("Executing Fields Request - Unable to decode/decrypt the content of the response!");
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    //no response from server
                    Connector.ReturnMessages.Add("Executing Fields Request -  Unable to get response from XPages!");
                    Connector.HasError = true;  //throws exception
                    return false;
                }
                return true;
            } else {
                Connector.ReturnMessages.Add("Executing Fields Request -  Unable to get the Document - Authentication Issue");
                Connector.HasError = true;  //throws exception
                return false;
            }
        } catch (Exception ex) {
            if (ex.GetType() == typeof(WebException)) {
                WebException webex;
                webex = (WebException)ex;
                if (webex.Response != null) {
                    Stream stream = webex.Response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        string errorResp = reader.ReadToEnd();
                        errorResp = errorResp.Replace("{", "").Replace("}", "").Replace("\\\\r\\\\n\\\\", Environment.NewLine).Replace(",", Environment.NewLine).Replace(((char)34).ToString(), "").Replace("\\\\r\\\\n", Environment.NewLine);
                        Connector.ReturnMessages.Add("Unable to Execute the Fields Request : Invalid request or not authenticated : " + Environment.NewLine + "Web Request Error Response : " + errorResp);
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    Connector.ReturnMessages.Add("Unable to Execute the Fields Request : " + Common.GetErrorInfo(ex));
                    Connector.HasError = true;  //throws exception
                    return false;
                }
            } else {
                Connector.ReturnMessages.Add("Unable to Execute the Fields Request : " + Common.GetErrorInfo(ex));
                Connector.HasError = true;  //throws exception
                return false;
            }
        }
    }

    protected internal bool ExecuteAllFilesRequest(string WebServiceURL, DatabaseObject dbObj) {
        HttpWebRequest request = null;

        try {
            if (!isInitialized) {
                Connector.ReturnMessages.Add("Unable to execute the All Files Request - Not Initialized/Connected To Server");
                Connector.HasError = true;
                return false;
            }

            //  ServicePointManager.Expect100Continue = false;

            //create an HTTP request

            request = (HttpWebRequest)WebRequest.Create(WebServiceURL + "?API_AllFiles");
            //request.Timeout = 300000; // set timeout to 5minutes
            request.Timeout = 1800000; //half an hour!
            request.CookieContainer = this.AuthenticationCookie;
            //  request.KeepAlive = True
            request.AllowAutoRedirect = true;

            request.Method = WebRequestMethods.Http.Post;   //"POST"
            request.ContentType = "application/jpi; charset=utf-8"; //application/json
                                                                    // Set the request stream
            string result;

            //add a identifier in the header to be use in JPI Service to check if the request is coming from a valid source aka JPI XPages Connector
            if (!AddIdentityHeader(ref request)) {
                Connector.ReturnMessages.Add("Unable to Execute All Files Request : Identity Header could not be added!");
                Connector.HasError = true;  //throws error
                return false;
            }

            //test with encryption
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("UserName: " + Connection.UserName);
            sb.AppendLine("ServerName: " + dbObj.ServerName);
            sb.AppendLine("FilePath: " + dbObj.FilePath);
            sb.AppendLine("ReplicationID: " + dbObj.ReplicationID);
            //sb.AppendLine("SearchField: " + searchField);
            sb.AppendLine("SearchField: ");
            //sb.AppendLine("SearchValue: " + searchValue);
            sb.AppendLine("SearchValue: ");
            //sb.AppendLine("Formula: " + formula);
            sb.AppendLine("Formula: ");
            string listUnids = String.Join(";", dbObj.Documents.Keys.ToArray());

            sb.AppendLine("Unids: " + listUnids);
            sb.AppendLine("Connecting via XPagesAPI");

            byte[] b = Encoding.Default.GetBytes(sb.ToString().Replace(Environment.NewLine, " | "));
            string myString = Encoding.UTF8.GetString(b);
            Encryptor EncodedEncryptedContent = new Encryptor(myString, true, ref Connection);
            if (EncodedEncryptedContent.Initialize()) {
                Stream requestStream = null;
                try {
                    requestStream = request.GetRequestStream();
                    using (StreamWriter writer = new StreamWriter(requestStream, Encoding.UTF8)) {
                        writer.Write(EncodedEncryptedContent.EncodedContent);
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the All Files Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (requestStream != null) {
                    //    requestStream.Dispose();
                    //}
                }
            } else {
                Connector.ReturnMessages.Add("Executing All Files Request - Unable to encode/encrypt the content of the request!");
                Connector.HasError = true; //throws exception
                return false;
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //can give webexception error 500

            if ((response.StatusCode == HttpStatusCode.OK) || (response.StatusCode == HttpStatusCode.Found) || (response.StatusCode == HttpStatusCode.Redirect) || (response.StatusCode == HttpStatusCode.Moved) || (response.StatusCode == HttpStatusCode.MovedPermanently)) {
                Stream responseStream = null;
                try {
                    responseStream = request.GetResponse().GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream, true)) {
                        result = reader.ReadToEnd();
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the All Files Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (responseStream != null) {
                    //    responseStream.Dispose();
                    //}
                }

                if (result != null) {
                    Encryptor DecodeDecryptedContent = new Encryptor(result, false, ref Connection);
                    if (DecodeDecryptedContent.Initialize()) {
                        ArrayList arList = new ArrayList();

                        string[] ar = null;
                        bool isError = false;

                        string message = "";
                        string details = "";
                        string value = "";
                        string[] sep = { "||" };
                        string[] NoteIds = { };
                        string[] Unids = { };
                        string[] Forms = { };
                        string[] Sizes = { };
                        string[] URLs = { };
                        string[] Created = { };
                        string[] Modified = { };
                        string[] Files = { };

                        ar = DecodeDecryptedContent.DecodedContent.Split(new[] { " | " }, StringSplitOptions.None);
                        arList.AddRange(ar);
                        foreach (string str in arList) {
                            //YES/NO
                            if (str.StartsWith("Error: ")) {
                                if (str.Replace("Error: ", "").Equals("Y", StringComparison.OrdinalIgnoreCase)) {
                                    isError = true;
                                }
                            }
                            if (str.StartsWith("Message: ")) {
                                message = str.Replace("Message: ", "");
                            }
                            if (str.StartsWith("Details: ")) {
                                details = str.Replace("Details: ", "");
                            }

                            if (str.StartsWith("Unids: ")) {
                                value = str.Replace("Unids: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Unids = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("NoteIDs: ")) {
                                value = str.Replace("NoteIDs: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    NoteIds = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("Forms: ")) {
                                value = str.Replace("Forms: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Forms = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("Sizes: ")) {
                                value = str.Replace("Sizes: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Sizes = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("URLs: ")) {
                                value = str.Replace("URLs: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    URLs = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("Created: ")) {
                                value = str.Replace("Created: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Created = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("Modified: ")) {
                                value = str.Replace("Modified: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Modified = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("FileObjects: ")) {
                                value = str.Replace("FileObjects: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Files = value.Split(sep, StringSplitOptions.None);
                                }
                            }
                        }

                        //loop through unids, notesids, forms, sizes, urls and create document objects
                        // check if all have the same number of items!
                        int count = 0;
                        if (Unids != null && Unids.Length > 0) {
                            count = Unids.Length;
                            if (NoteIds != null && NoteIds.Length > 0) {
                                if (count != NoteIds.Length) {
                                    Connector.ReturnMessages.Add("Executing All Files Request -  Unable to get the Documents - Unids <> NoteIds");
                                    Connector.HasError = true;  //throws exception
                                    return false;
                                } else {
                                    // form can be empty!!
                                    //if (Forms != null && Forms.Length > 0) {
                                    //    if (count != Forms.Length) {
                                    //        Connector.ReturnMessages.Add("Executing All Documents Request -  Unable to get the Documents - Unids <> Forms");
                                    //        Connector.hasError = true;  //throws exception
                                    //        return false;
                                    //    } else {
                                    if (Sizes != null && Sizes.Length > 0) {
                                        if (count != Sizes.Length) {
                                            Connector.ReturnMessages.Add("Executing All Files Request -  Unable to get the Documents - Unids <> Sizes");
                                            Connector.HasError = true;  //throws exception
                                            return false;
                                        } else {
                                            if (URLs != null && URLs.Length > 0) {
                                                if (count != URLs.Length) {
                                                    Connector.ReturnMessages.Add("Executing All Files Request -  Unable to get the Documents - Unids <> URLs");
                                                    Connector.HasError = true;  //throws exception
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                    //   }
                                    // }
                                }
                            }
                            // loop through and create doc obj
                            DocumentObject docObj = null;
                            if (dbObj.Documents == null) {
                                dbObj.Documents = new Dictionary<string, DocumentObject>();
                            }
                            for (int i = 0; i < Unids.Length; i++) {
                                docObj = new DocumentObject(dbObj, Unids[i]);
                                if (NoteIds.Length - 1 >= i) {
                                    docObj.NoteID = NoteIds[i];
                                }
                                if (Forms.Length - 1 >= i) {
                                    docObj.Form = Forms[i];
                                }
                                if (Sizes.Length - 1 >= i) {
                                    docObj.Size = Sizes[i];
                                }
                                if (URLs.Length - 1 >= i) {
                                    docObj.Url = URLs[i];
                                }
                                if (Created.Length - 1 >= i) {
                                    docObj.DateCreated = Created[i];
                                }
                                if (Modified.Length - 1 >= i) {
                                    docObj.DateModified = Modified[i];
                                }
                                if (Files.Length - 1 >= i) {
                                    //docObj.DateModified = Files[i];
                                    //get all files into fileobjects here - split on 'FileObject: '
                                    docObj.Files = new SortedDictionary<string, FileObject>();
                                    FileObject fObj = null;
                                    string[] arFiles = Files[i].Split(new[] { "FileObject: " }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (string str in arFiles) {
                                        if (!string.IsNullOrEmpty(str) && !str.Contains("<NO_FILES_ATTACHED>")) {
                                            //can be valid file here split on § and count
                                            //fObj.Application + "§" + fObj.Creator + "§" + fObj.DateCreated + "§" + fObj.DateModfied + "$" + fObj.FieldName + "§" + fObj.FileExtension + "§" + fObj.FileName + "§" + fObj.FileSize + "§" + fObj.LinkToFile + "§" + fObj.Other + "§" + fObj.SoftClass);
                                            fObj = new FileObject(docObj);
                                            if (fObj.Initialize(str)) {
                                                docObj.Files.Add(fObj.Name, fObj);
                                            }
                                        }
                                    }
                                }
                                if (dbObj.Documents.ContainsKey(Unids[i])) {
                                    dbObj.Documents.Remove(Unids[i]);
                                }
                                docObj.IsInitialized = true;
                                dbObj.Documents.Add(Unids[i], docObj);
                            }
                        }

                        if (isError) {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = true; //throws exception
                            return false;
                        } else {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = false; //SUCCESS
                        }
                    } else {
                        Connector.ReturnMessages.Add("Executing All Files Request - Unable to decode/decrypt the content of the response!");
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    //no response from server
                    Connector.ReturnMessages.Add("Executing All Files Request -  Unable to get response from XPages!");
                    Connector.HasError = true;  //throws exception
                    return false;
                }
                return true;
            } else {
                Connector.ReturnMessages.Add("Executing All Files Request -  Unable to get the Document - Authentication Issue");
                Connector.HasError = true;  //throws exception
                return false;
            }
        } catch (Exception ex) {
            if (ex.GetType() == typeof(WebException)) {
                WebException webex;
                webex = (WebException)ex;
                if (webex.Response != null) {
                    Stream stream = webex.Response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        string errorResp = reader.ReadToEnd();
                        errorResp = errorResp.Replace("{", "").Replace("}", "").Replace("\\\\r\\\\n\\\\", Environment.NewLine).Replace(",", Environment.NewLine).Replace(((char)34).ToString(), "").Replace("\\\\r\\\\n", Environment.NewLine);
                        Connector.ReturnMessages.Add("Unable to Execute the All Files Request : Invalid request or not authenticated : " + Environment.NewLine + "Web Request Error Response : " + errorResp);
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    Connector.ReturnMessages.Add("Unable to Execute the All Files Request : " + Common.GetErrorInfo(ex));
                    Connector.HasError = true;  //throws exception
                    return false;
                }
            } else {
                Connector.ReturnMessages.Add("Unable to Execute the All Files Request : " + Common.GetErrorInfo(ex));
                Connector.HasError = true;  //throws exception
                return false;
            }
        }
    }

    protected internal bool ExecuteAllDocumentsFilesFieldsRequest(string WebServiceURL, string searchField, string searchValue, string formula, string unids, DatabaseObject dbObj, string fields) {
        HttpWebRequest request = null;

        try {
            if (!isInitialized) {
                Connector.ReturnMessages.Add("Unable to execute the All Files Fields  Request - Not Initialized/Connected To Server");
                Connector.HasError = true;
                return false;
            }

            //  ServicePointManager.Expect100Continue = false;

            //create an HTTP request

            request = (HttpWebRequest)WebRequest.Create(WebServiceURL + "?API_AllFilesFields");
            //request.Timeout = 300000; // set timeout to 5minutes
            request.Timeout = 1800000; //half an hour!
            request.CookieContainer = this.AuthenticationCookie;
            //  request.KeepAlive = True
            request.AllowAutoRedirect = true;

            request.Method = WebRequestMethods.Http.Post;   //"POST"
            request.ContentType = "application/jpi; charset=utf-8"; //application/json
                                                                    // Set the request stream
            string result;

            //add a identifier in the header to be use in JPI Service to check if the request is coming from a valid source aka JPI XPages Connector
            if (!AddIdentityHeader(ref request)) {
                Connector.ReturnMessages.Add("Unable to Execute All Files Fields Request : Identity Header could not be added!");
                Connector.HasError = true;  //throws error
                return false;
            }

            //test with encryption
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("UserName: " + Connection.UserName);
            sb.AppendLine("ServerName: " + dbObj.ServerName);
            sb.AppendLine("FilePath: " + dbObj.FilePath);
            sb.AppendLine("ReplicationID: " + dbObj.ReplicationID);
            sb.AppendLine("SearchField: " + searchField);
            sb.AppendLine("SearchValue: " + searchValue);
            sb.AppendLine("Formula: " + formula);
            sb.AppendLine("Unids: " + unids);
            sb.AppendLine("Fields: " + fields);

            sb.AppendLine("Connecting via XPagesAPI");

            byte[] b = Encoding.Default.GetBytes(sb.ToString().Replace(Environment.NewLine, " | "));
            string myString = Encoding.UTF8.GetString(b);
            Encryptor EncodedEncryptedContent = new Encryptor(myString, true, ref Connection);
            if (EncodedEncryptedContent.Initialize()) {
                Stream requestStream = null;
                try {
                    requestStream = request.GetRequestStream();
                    using (StreamWriter writer = new StreamWriter(requestStream, Encoding.UTF8)) {
                        writer.Write(EncodedEncryptedContent.EncodedContent);
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the All Files Fields  Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (requestStream != null) {
                    //    requestStream.Dispose();
                    //}
                }
            } else {
                Connector.ReturnMessages.Add("Executing All Files Fields Request - Unable to encode/encrypt the content of the request!");
                Connector.HasError = true; //throws exception
                return false;
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //can give webexception error 500

            if ((response.StatusCode == HttpStatusCode.OK) || (response.StatusCode == HttpStatusCode.Found) || (response.StatusCode == HttpStatusCode.Redirect) || (response.StatusCode == HttpStatusCode.Moved) || (response.StatusCode == HttpStatusCode.MovedPermanently)) {
                Stream responseStream = null;
                try {
                    responseStream = request.GetResponse().GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream, true)) {
                        result = reader.ReadToEnd();
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the All Files Fields Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (responseStream != null) {
                    //    responseStream.Dispose();
                    //}
                }

                if (result != null) {
                    Encryptor DecodeDecryptedContent = new Encryptor(result, false, ref Connection);
                    if (DecodeDecryptedContent.Initialize()) {
                        ArrayList arList = new ArrayList();

                        string[] ar = null;
                        bool isError = false;
                        string unid = "";

                        string message = "";
                        string details = "";
                        string value = "";
                        string[] sep = { "||" };
                        string[] sepFields = { "§§" };
                        string[] sepFieldNames = { "@FieldNames@: " };
                        string[] sepFieldValues = { "@FieldValues@: " };
                        string[] sepFieldTypes = { "@FieldTypes@: " };
                        string[] NoteIds = { };
                        string[] Unids = { };
                        string[] Forms = { };
                        string[] Sizes = { };
                        string[] URLs = { };
                        string[] Created = { };
                        string[] Modified = { };
                        string[] Files = { };

                        Dictionary<string, List<string[]>> dict = new Dictionary<string, List<string[]>>();
                        string[] fieldnames = null;
                        string[] fieldvalues = null;
                        string[] fieldtypes = null;

                       
                        ar = DecodeDecryptedContent.DecodedContent.Split(new[] { " | " }, StringSplitOptions.None);
                        arList.AddRange(ar);
                        foreach (string str in arList) {
                            //YES/NO
                            if (str.StartsWith("Error: ")) {
                                if (str.Replace("Error: ", "").Equals("Y", StringComparison.OrdinalIgnoreCase)) {
                                    isError = true;
                                }
                            }
                            if (str.StartsWith("Message: ")) {
                                message = str.Replace("Message: ", "");
                            }
                            if (str.StartsWith("Details: ")) {
                                details = str.Replace("Details: ", "");
                            }

                            if (str.StartsWith("Unids: ")) {
                                value = str.Replace("Unids: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Unids = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("NoteIDs: ")) {
                                value = str.Replace("NoteIDs: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    NoteIds = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("Forms: ")) {
                                value = str.Replace("Forms: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Forms = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("Sizes: ")) {
                                value = str.Replace("Sizes: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Sizes = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("URLs: ")) {
                                value = str.Replace("URLs: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    URLs = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("Created: ")) {
                                value = str.Replace("Created: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Created = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            if (str.StartsWith("Modified: ")) {
                                value = str.Replace("Modified: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Modified = value.Split(sep, StringSplitOptions.None);
                                }
                            }

                            // 'FieldNames: Unid: '
                            if (str.StartsWith("@@ListFieldNames: ")) {
                                value = str.Replace("@@ListFieldNames: ", "");

                                if (!string.IsNullOrEmpty(value)) {
                                    fieldnames = value.Split(sepFieldNames, StringSplitOptions.RemoveEmptyEntries);
                                }
                             
                            }

                            if (str.StartsWith("@@ListFieldValues: ")) {
                                value = str.Replace("@@ListFieldValues: ", "");

                                if (!string.IsNullOrEmpty(value)) {
                                    fieldvalues = value.Split(sepFieldValues, StringSplitOptions.RemoveEmptyEntries);
                                }
                            }

                            if (str.StartsWith("@@ListFieldTypes: ")) {
                                value = str.Replace("@@ListFieldTypes: ", "");

                                if (!string.IsNullOrEmpty(value)) {
                                    fieldtypes = value.Split(sepFieldTypes, StringSplitOptions.RemoveEmptyEntries);
                                }
                            }

                            if (str.StartsWith("FileObjects: ")) {
                                value = str.Replace("FileObjects: ", "");
                                if (!string.IsNullOrEmpty(value)) {
                                    Files = value.Split(sep, StringSplitOptions.None);
                                }
                            }
                        }

                        //loop through unids, notesids, forms, sizes, urls and create document objects
                        // check if all have the same number of items!
                        int count = 0;
                        if (Unids != null && Unids.Length > 0) {
                            count = Unids.Length;
                            if (NoteIds != null && NoteIds.Length > 0) {
                                if (count != NoteIds.Length) {
                                    Connector.ReturnMessages.Add("Executing All Files Request -  Unable to get the Documents - Unids <> NoteIds");
                                    Connector.HasError = true;  //throws exception
                                    return false;
                                } else {
                                    
                                    if (Sizes != null && Sizes.Length > 0) {
                                        if (count != Sizes.Length) {
                                            Connector.ReturnMessages.Add("Executing All Files Request -  Unable to get the Documents - Unids <> Sizes");
                                            Connector.HasError = true;  //throws exception
                                            return false;
                                        } else {
                                            if (URLs != null && URLs.Length > 0) {
                                                if (count != URLs.Length) {
                                                    Connector.ReturnMessages.Add("Executing All Files Request -  Unable to get the Documents - Unids <> URLs");
                                                    Connector.HasError = true;  //throws exception
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                    //   }
                                    // }
                                }
                            }
                            // loop through and create doc obj
                            DocumentObject docObj = null;
                            if (dbObj.Documents == null) {
                                dbObj.Documents = new Dictionary<string, DocumentObject>();
                            }
                            for (int i = 0; i < Unids.Length; i++) {
                                docObj = new DocumentObject(dbObj, Unids[i]);
                                if (NoteIds.Length - 1 >= i) {
                                    docObj.NoteID = NoteIds[i];
                                }
                                if (Forms.Length - 1 >= i) {
                                    docObj.Form = Forms[i];
                                }
                                if (Sizes.Length - 1 >= i) {
                                    docObj.Size = Sizes[i];
                                }
                                if (URLs.Length - 1 >= i) {
                                    docObj.Url = URLs[i];
                                }
                                if (Created.Length - 1 >= i) {
                                    docObj.DateCreated = Created[i];
                                }
                                if (Modified.Length - 1 >= i) {
                                    docObj.DateModified = Modified[i];
                                }
                                if (Files.Length - 1 >= i) {
                                    //docObj.DateModified = Files[i];
                                    //get all files into fileobjects here - split on 'FileObject: '
                                    docObj.Files = new SortedDictionary<string, FileObject>();
                                    FileObject fileObj = null;
                                    string[] arFiles = Files[i].Split(new[] { "§§FileObject: " }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (string str in arFiles) {
                                        if (!string.IsNullOrEmpty(str) && !str.Contains("<NO_FILES_ATTACHED>")) {
                                            //can be valid file here split on § and count
                                            //fObj.Application + "§" + fObj.Creator + "§" + fObj.DateCreated + "§" + fObj.DateModfied + "$" + fObj.FieldName + "§" + fObj.FileExtension + "§" + fObj.FileName + "§" + fObj.FileSize + "§" + fObj.LinkToFile + "§" + fObj.Other + "§" + fObj.SoftClass);
                                            fileObj = new FileObject(docObj);
                                            if (fileObj.Initialize(str)) {
                                                docObj.Files.Add(fileObj.Name, fileObj);
                                            }
                                        }
                                    }
                                }

                                if (fieldnames.Length - 1 >= i) {

                                    // remove unid here

                                    //get the unid C125760F0054EEFBC1257356002F192D
                                    //split the values here in @FieldNames@
                                    value = "";
                                    value = fieldnames[i];

                                    if (value.Length > 34) { //Unid is 32 + ': '
                                        unid = value.Substring(0, 32);
                                        //remove unid
                                        value = value.Remove(0, 34);
                                        if (!string.IsNullOrEmpty(value)) {
                                            string[] listfieldnames = value.Split(sepFields, StringSplitOptions.None);
                                            // check if item is already in dict
                                            if (!dict.ContainsKey(unid)) {
                                                dict.Add(unid, new List<string[]>());
                                            }
                                            dict[unid].Add(listfieldnames);
                                        }
                                    }// else something is wrong

                                }

                                if (fieldvalues.Length - 1 >= i) {

                                    // remove unid here

                                    //get the unid C125760F0054EEFBC1257356002F192D
                                    //split the values here in @FieldNames@
                                    value = "";
                                    value = fieldvalues[i];

                                    if (value.Length > 34) { //Unid is 32 + ': '
                                        unid = value.Substring(0, 32);
                                        //remove unid
                                        value = value.Remove(0, 34);
                                        if (!string.IsNullOrEmpty(value)) {
                                            string[] listfieldvalues = value.Split(sepFields, StringSplitOptions.None);
                                            // check if item is already in dict
                                            if (!dict.ContainsKey(unid)) {
                                                dict.Add(unid, new List<string[]>());
                                            }
                                            dict[unid].Add(listfieldvalues);
                                        }
                                    }// else something is wrong

                                }

                                if (fieldtypes.Length - 1 >= i) {

                                    // remove unid here

                                    //get the unid C125760F0054EEFBC1257356002F192D
                                    //split the values here in @FieldNames@
                                    value = "";
                                    value = fieldtypes[i];

                                    if (value.Length > 34) { //Unid is 32 + ': '
                                        unid = value.Substring(0, 32);
                                        //remove unid
                                        value = value.Remove(0, 34);
                                        if (!string.IsNullOrEmpty(value)) {
                                            string[] listfieldtypes = value.Split(sepFields, StringSplitOptions.None);
                                            // check if item is already in dict
                                            if (!dict.ContainsKey(unid)) {
                                                dict.Add(unid, new List<string[]>());
                                            }
                                            dict[unid].Add(listfieldtypes);
                                        }
                                    }// else something is wrong

                                }

                                if (dbObj.Documents.ContainsKey(Unids[i])) {
                                    dbObj.Documents.Remove(Unids[i]);
                                }
                                docObj.IsInitialized = true;
                                dbObj.Documents.Add(Unids[i], docObj);
                            }


                            docObj = null;
                            FieldObject fObj = null;

                            foreach (KeyValuePair<string, List<string[]>> kvp in dict) {
                                if (dbObj.Documents.ContainsKey(kvp.Key)) {
                                    docObj = dbObj.Documents[kvp.Key];
                                    if (docObj.Fields == null) {
                                        docObj.Fields = new SortedDictionary<string, FieldObject>();
                                    }
                                    //reset
                                    fieldnames = null;
                                    fieldvalues = null;
                                    fieldtypes = null;
                                    // first array in list should be fieldnames, then values then types
                                    fieldnames = kvp.Value[0];
                                    fieldvalues = kvp.Value[1];
                                    fieldtypes = kvp.Value[2];

                                    for (int i = 0; i < fieldnames.Length; i++) {//fieldnames
                                        fObj = new FieldObject(fieldnames[i]);
                                        if (fieldvalues.Length - 1 >= i) {
                                            fObj.Value = fieldvalues[i];
                                        }
                                        if (fieldtypes.Length - 1 >= i) {
                                            fObj.Type = fieldtypes[i];
                                        }

                                        if (docObj.Fields.ContainsKey(fObj.Name)) {
                                            docObj.Fields.Remove(fObj.Name);
                                        }
                                        docObj.Fields.Add(fObj.Name, fObj);
                                    }

                                    if (dbObj.Documents.ContainsKey(kvp.Key)) {
                                        dbObj.Documents.Remove(kvp.Key);
                                    }
                                    docObj.IsInitialized = true;
                                    dbObj.Documents.Add(kvp.Key, docObj);

                                } else {
                                    Connector.ReturnMessages.Add("Unable to find the document in the databaseobject :" + kvp.Key);
                                    isError = true;
                                }
                            }
                        }

                        if (isError) {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = true; //throws exception
                            return false;
                        } else {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = false; //SUCCESS
                        }
                    } else {
                        Connector.ReturnMessages.Add("Executing All Files Request - Unable to decode/decrypt the content of the response!");
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    //no response from server
                    Connector.ReturnMessages.Add("Executing All Files Request -  Unable to get response from XPages!");
                    Connector.HasError = true;  //throws exception
                    return false;
                }
                return true;
            } else {
                Connector.ReturnMessages.Add("Executing All Files Request -  Unable to get the Document - Authentication Issue");
                Connector.HasError = true;  //throws exception
                return false;
            }
        } catch (Exception ex) {
            if (ex.GetType() == typeof(WebException)) {
                WebException webex;
                webex = (WebException)ex;
                if (webex.Response != null) {
                    Stream stream = webex.Response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        string errorResp = reader.ReadToEnd();
                        errorResp = errorResp.Replace("{", "").Replace("}", "").Replace("\\\\r\\\\n\\\\", Environment.NewLine).Replace(",", Environment.NewLine).Replace(((char)34).ToString(), "").Replace("\\\\r\\\\n", Environment.NewLine);
                        Connector.ReturnMessages.Add("Unable to Execute the All Files Request : Invalid request or not authenticated : " + Environment.NewLine + "Web Request Error Response : " + errorResp);
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    Connector.ReturnMessages.Add("Unable to Execute the All Files Request : " + Common.GetErrorInfo(ex));
                    Connector.HasError = true;  //throws exception
                    return false;
                }
            } else {
                Connector.ReturnMessages.Add("Unable to Execute the All Files Request : " + Common.GetErrorInfo(ex));
                Connector.HasError = true;  //throws exception
                return false;
            }
        }
    }

    protected internal bool ExecuteAllFieldsRequest(string WebServiceURL, DatabaseObject dbObj, string fields) {
        HttpWebRequest request = null;

        try {
            if (!isInitialized) {
                Connector.ReturnMessages.Add("Unable to execute the Fields Request - Not Initialized/Connected To Server");
                Connector.HasError = true;
                return false;
            }

            // ServicePointManager.Expect100Continue = false;

            //create an HTTP request

            request = (HttpWebRequest)WebRequest.Create(WebServiceURL + "?API_AllFields");
            // request.Timeout = 300000; // set timeout to 5minutes
            request.Timeout = 1800000; //half an hour!
            request.CookieContainer = this.AuthenticationCookie;
            //  request.KeepAlive = True
            request.AllowAutoRedirect = true;

            request.Method = WebRequestMethods.Http.Post;   //"POST"
            request.ContentType = "application/jpi; charset=utf-8"; //application/json
                                                                    // Set the request stream
            string result;

            //add a identifier in the header to be use in JPI Service to check if the request is coming from a valid source aka JPI XPages Connector
            if (!AddIdentityHeader(ref request)) {
                Connector.ReturnMessages.Add("Unable to Execute Fields Request : Identity Header could not be added!");
                Connector.HasError = true;  //throws error
                return false;
            }

            //test with encryption
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("UserName: " + Connection.UserName);
            sb.AppendLine("ServerName: " + dbObj.ServerName);
            sb.AppendLine("FilePath: " + dbObj.FilePath);
            sb.AppendLine("ReplicationID: " + dbObj.ReplicationID);
            string listUnids = String.Join(";", dbObj.Documents.Keys.ToArray());

            //foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
            //    listUnids = listUnids + kvp.Key + ";";
            //}
            //listUnids = listUnids.Substring(0, listUnids.Length - 1); // remove ; at the end

            sb.AppendLine("UniversalIDs: " + listUnids);
            sb.AppendLine("Fields: " + fields);

            sb.AppendLine("Connecting via XPagesAPI");

            byte[] b = Encoding.Default.GetBytes(sb.ToString().Replace(Environment.NewLine, " | "));
            string myString = Encoding.UTF8.GetString(b);
            Encryptor EncodedEncryptedContent = new Encryptor(myString, true, ref Connection);
            if (EncodedEncryptedContent.Initialize()) {
                Stream requestStream = null;
                try {
                    requestStream = request.GetRequestStream();
                    using (StreamWriter writer = new StreamWriter(requestStream, Encoding.UTF8)) {
                        writer.Write(EncodedEncryptedContent.EncodedContent);
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the All Fields Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (requestStream != null) {
                    //    requestStream.Dispose();
                    //}
                }
            } else {
                Connector.ReturnMessages.Add("Executing All Fields Request - Unable to encode/encrypt the content of the request!");
                Connector.HasError = true; //throws exception
                return false;
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //can give webexception error 500

            if ((response.StatusCode == HttpStatusCode.OK) || (response.StatusCode == HttpStatusCode.Found) || (response.StatusCode == HttpStatusCode.Redirect) || (response.StatusCode == HttpStatusCode.Moved) || (response.StatusCode == HttpStatusCode.MovedPermanently)) {
                Stream responseStream = null;
                try {
                    responseStream = request.GetResponse().GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream, true)) {
                        result = reader.ReadToEnd();
                    }
                } catch (Exception e) {
                    Connector.ReturnMessages.Add("Unable to execute the All Fields Request : " + Common.GetErrorInfo(e));
                    Connector.HasError = true;  //throws exception
                    return false;
                } finally {
                    //if (responseStream != null) {
                    //    responseStream.Dispose();
                    //}
                }

                if (result != null) {
                    Encryptor DecodeDecryptedContent = new Encryptor(result, false, ref Connection);
                    if (DecodeDecryptedContent.Initialize()) {
                        ArrayList arList = new ArrayList();

                        string[] ar = null;
                        bool isError = false;

                        string message = "";
                        string details = "";
                        string value = "";
                        string unid = "";
                        Dictionary<string, List<string[]>> dict = new Dictionary<string, List<string[]>>();
                        string[] fieldnames = null;
                        string[] fieldvalues = null;
                        string[] fieldtypes = null;

                        string[] sep = { "||" };

                        ar = DecodeDecryptedContent.DecodedContent.Split(new[] { " | " }, StringSplitOptions.None);
                        arList.AddRange(ar);
                        foreach (string str in arList) {
                            //YES/NO
                            if (str.StartsWith("Error: ")) {
                                if (str.Replace("Error: ", "").Equals("Y", StringComparison.OrdinalIgnoreCase)) {
                                    isError = true;
                                }
                            }
                            if (str.StartsWith("Message: ")) {
                                message = str.Replace("Message: ", "");
                            }
                            if (str.StartsWith("Details: ")) {
                                details = str.Replace("Details: ", "");
                            }

                            // 'FieldNames: Unid: '
                            if (str.StartsWith("FieldNames: ")) {
                                value = str.Replace("FieldNames: ", "");
                                //get the unid C125760F0054EEFBC1257356002F192D
                                if (value.Length > 34) { //Unid is 32 + ': '
                                    unid = value.Substring(0, 32);
                                    //remove unid
                                    value = value.Remove(0, 34);
                                    if (!string.IsNullOrEmpty(value)) {
                                        fieldnames = value.Split(sep, StringSplitOptions.None);
                                        // check if item is already in dict
                                        if (!dict.ContainsKey(unid)) {
                                            dict.Add(unid, new List<string[]>());
                                        }
                                        dict[unid].Add(fieldnames);
                                    }
                                }// else something is wrong
                            }

                            if (str.StartsWith("FieldValues: ")) {
                                value = str.Replace("FieldValues: ", "");
                                if (value.Length > 34) { //Unid is 32 + ': '
                                    unid = value.Substring(0, 32);
                                    //remove unid
                                    value = value.Remove(0, 34);
                                    if (!string.IsNullOrEmpty(value)) {
                                        fieldvalues = value.Split(sep, StringSplitOptions.None);
                                        // check if item is already in dict
                                        if (!dict.ContainsKey(unid)) {
                                            dict.Add(unid, new List<string[]>());
                                        }
                                        dict[unid].Add(fieldvalues);
                                    }
                                }
                            }

                            if (str.StartsWith("FieldTypes: ")) {
                                value = str.Replace("FieldTypes: ", "");
                                if (value.Length > 34) { //Unid is 32 + ': '
                                    unid = value.Substring(0, 32);
                                    //remove unid
                                    value = value.Remove(0, 34);
                                    if (!string.IsNullOrEmpty(value)) {
                                        fieldtypes = value.Split(sep, StringSplitOptions.None);
                                        // check if item is already in dict
                                        if (!dict.ContainsKey(unid)) {
                                            dict.Add(unid, new List<string[]>());
                                        }
                                        dict[unid].Add(fieldtypes);
                                    }
                                }
                            }
                        }

                        DocumentObject docObj = null;
                        FieldObject fObj = null;

                        foreach (KeyValuePair<string, List<string[]>> kvp in dict) {
                            if (dbObj.Documents.ContainsKey(kvp.Key)) {
                                docObj = dbObj.Documents[kvp.Key];
                                if (docObj.Fields == null) {
                                    docObj.Fields = new SortedDictionary<string, FieldObject>();
                                }
                                //reset
                                fieldnames = null;
                                fieldvalues = null;
                                fieldtypes = null;
                                // first array in list should be fieldnames, then values then types
                                fieldnames = kvp.Value[0];
                                fieldvalues = kvp.Value[1];
                                fieldtypes = kvp.Value[2];

                                for (int i = 0; i < fieldnames.Length; i++) {//fieldnames
                                    fObj = new FieldObject(fieldnames[i]);
                                    if (fieldvalues.Length - 1 >= i) {
                                        fObj.Value = fieldvalues[i];
                                    }
                                    if (fieldtypes.Length - 1 >= i) {
                                        fObj.Type = fieldtypes[i];
                                    }

                                    if (docObj.Fields.ContainsKey(fObj.Name)) {
                                        docObj.Fields.Remove(fObj.Name);
                                    }
                                    docObj.Fields.Add(fObj.Name, fObj);
                                }
                            } else {
                                Connector.ReturnMessages.Add("Unable to find the document in the databaseobject :" + kvp.Key);
                                isError = true;
                            }
                        }

                        if (isError) {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = true; //throws exception
                            return false;
                        } else {
                            Connector.ReturnMessages.Add(message);
                            Connector.ReturnMessages.Add(details);
                            Connector.HasError = false; //SUCCESS
                        }
                    } else {
                        Connector.ReturnMessages.Add("Executing Fields Request - Unable to decode/decrypt the content of the response!");
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    //no response from server
                    Connector.ReturnMessages.Add("Executing Fields Request -  Unable to get response from XPages!");
                    Connector.HasError = true;  //throws exception
                    return false;
                }
                return true;
            } else {
                Connector.ReturnMessages.Add("Executing Fields Request -  Unable to get the Document - Authentication Issue");
                Connector.HasError = true;  //throws exception
                return false;
            }
        } catch (Exception ex) {
            if (ex.GetType() == typeof(WebException)) {
                WebException webex;
                webex = (WebException)ex;
                if (webex.Response != null) {
                    Stream stream = webex.Response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        string errorResp = reader.ReadToEnd();
                        errorResp = errorResp.Replace("{", "").Replace("}", "").Replace("\\\\r\\\\n\\\\", Environment.NewLine).Replace(",", Environment.NewLine).Replace(((char)34).ToString(), "").Replace("\\\\r\\\\n", Environment.NewLine);
                        Connector.ReturnMessages.Add("Unable to Execute the All Fields Request : Invalid request or not authenticated : " + Environment.NewLine + "Web Request Error Response : " + errorResp);
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    Connector.ReturnMessages.Add("Unable to Execute the All Fields Request : " + Common.GetErrorInfo(ex));
                    Connector.HasError = true;  //throws exception
                    return false;
                }
            } else {
                Connector.ReturnMessages.Add("Unable to Execute the All Fields Request : " + Common.GetErrorInfo(ex));
                Connector.HasError = true;  //throws exception
                return false;
            }
        }
    }

    protected internal bool ExecuteGetFileRequest(string FileURL, string LocalFilePath) {
        HttpWebRequest request = null;
        try {
            if (!isInitialized) {
                Connector.ReturnMessages.Add("Unable to execute the Get File Request - Not Initialized/Connected To Server");
                Connector.HasError = true;
                return false;
            }

            request = request = (HttpWebRequest)WebRequest.Create(FileURL);

            request.CookieContainer = AuthenticationCookie;
            request.AllowAutoRedirect = true;
            request.Method = WebRequestMethods.Http.Get;
            const int BUFFER_SIZE = 16 * 1024;
            byte[] buffer = new byte[BUFFER_SIZE - 1];
            int bytesRead;

            using (var outputFileStream = File.Create(LocalFilePath, BUFFER_SIZE)) {
                using (var response2 = request.GetResponse()) {
                    using (var responseStream = response2.GetResponseStream()) {
                        while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0) {
                            outputFileStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
            return true;
        } catch (Exception ex) {
            if (ex.GetType() == typeof(WebException)) {
                WebException webex;
                webex = (WebException)ex;
                if (webex.Response != null) {
                    Stream stream = webex.Response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        string errorResp = reader.ReadToEnd();
                        errorResp = errorResp.Replace("{", "").Replace("}", "").Replace("\\\\r\\\\n\\\\", Environment.NewLine).Replace(",", Environment.NewLine).Replace(((char)34).ToString(), "").Replace("\\\\r\\\\n", Environment.NewLine);
                        Connector.ReturnMessages.Add("Unable to Execute the Get File Request : Invalid request or not authenticated : " + Environment.NewLine + "Web Request Error Response : " + errorResp);
                        Connector.HasError = true;  //throws exception
                        return false;
                    }
                } else {
                    Connector.ReturnMessages.Add("Unable to Execute the Get File Request : " + Common.GetErrorInfo(ex));
                    Connector.HasError = true;  //throws exception
                    return false;
                }
            } else {
                Connector.ReturnMessages.Add("Unable to Execute the Get File Request : " + Common.GetErrorInfo(ex));
                Connector.HasError = true;  //throws exception
                return false;
            }
        }
    }

    #endregion Protected Internal Methods
}