﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace APITesting {

    [TestClass]
    public class API_UnitTesting {
        private TestContext testContextInstance;

        // Test intranet connection (http)
        private const string ServerURL = "http://antln-test.europe.jacobs.com";
        private const string UserName = "Kim Acket";
        private const string Password = "Je082018";

        // Test internet connection (https)
        //private const string ServerURL = "https://jpi2.jacobs.com";
        //private const string UserName = "Kim Acket";
        //private const string Password = "Lambam1608";

        // XPages JPI Service URL on intranet (http)
        // private const string XPagesServiceURL = "http://antln-test.europe.jacobs.com/projects/jpix/XPagesAPI_Interface.nsf/xpJPIService.xsp/JPIService";
        private const string XPagesServiceURL = "http://antln-test.europe.jacobs.com/projects/jpix/XPageDev1300_XP.nsf/xpJPIService.xsp/JPIService";

        // XPages JPI Service URL on internet (https)
        //private const string XPagesServiceURL = "https://jpi2.jacobs.com/projects/jpix/XPageDev1300_XP.nsf/xpJPIService.xsp/JPIService";

        //Database To Access
        private const string DatabaseFilePath = "projects\\jpi4\\XP2015_JP.nsf";

        //Server To Access
        private const string DominoServerName = "ANTLN-TEST/ANTWERPEN/JacobsEngineering";

        //private const string DominoServerName = "JPI2/JPI";

        private const string fieldNameList = "jeNumber;jeClientNumber;StatusCode; StatusTitle;jeTitle1;jeTitle2;jeTitle3;jeTitle4;jeTitle5;wfRevisionCode;wfRevisionDate_Text";

        /// <summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestInitialize]
        public void Initialize() {
            TestContext.WriteLine("Initialize...");
            if (System.IO.Directory.Exists("C:\\Users\\Acketk\\source\\repos\\VSTS\\XPagesAPI\\TestResults\\Files")) {
                TestContext.WriteLine("Removing Directories & Files : C:\\Users\\Acketk\\source\\repos\\VSTS\\XPagesAPI\\TestResults\\Files");
                System.IO.Directory.Delete("C:\\Users\\Acketk\\source\\repos\\VSTS\\XPagesAPI\\TestResults\\Files", true);
            }
            System.IO.Directory.CreateDirectory("C:\\Users\\Acketk\\source\\repos\\VSTS\\XPagesAPI\\TestResults\\Files");
            //// clean output folder
            //string[] result = System.IO.Directory.GetFiles("C:\\Users\\Acketk\\source\\repos\\VSTS\\XPagesAPI\\TestResults\\Files");
            //if(result!=null && result.Length > 0) {
            //    foreach (string str in result) {
            //        TestContext.WriteLine("Removing : " + str);
            //        System.IO.File.Delete(str);
            //    }
            //}

        }

        [TestMethod]
        public void ConnectorTesting() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected

                    // Assert.AreEqual(true, connector.Connect());
                    if (connector.Connect()) {
                        TestContext.WriteLine("Connected to JPI");
                        Assert.AreEqual(true, connector.IsConnected);
                    } else {
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                Assert.Fail("Error : " + ex.Message);
            }
        }

        [TestMethod]
        public void SessionTesting() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Test OK!
                            TestContext.WriteLine("Session Retrieved");
                            Assert.AreEqual(true, session.IsInitialized);
                        }
                    } else {
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                Assert.Fail("Error : " + ex.Message);
            }
        }

        [TestMethod]
        public void DatabaseTesting() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void AllDatabasesTesting() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve all Databases via the session object - provide the servername
                            if (session.GetAllDatabases(DominoServerName) && !Connector.HasError) {
                                // This function will add the found database in the 'Databases' property of the session object
                                if (session.Databases != null && session.Databases.Count > 0) {
                                    DatabaseObject dbObj = null;
                                    TestContext.WriteLine("All DatabaseObjects Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    TestContext.WriteLine("");
                                    foreach (KeyValuePair<string, DatabaseObject> kvp in session.Databases) {
                                        dbObj = kvp.Value;
                                        TestContext.WriteLine("FilePath : " + dbObj.FilePath + Environment.NewLine + "Server : " + dbObj.ServerName + Environment.NewLine + "Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                        TestContext.WriteLine("");
                                    }
                                    Assert.AreEqual(true, session.Databases != null && session.Databases.Count > 0);
                                } else {
                                    // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    Assert.Fail("DatabaseObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void DocumentRetrievalTestingByUnid() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the document by providing the unid - this function will call documentobject initialize and the isInitialized flag is set
                                docObj = dbObj.GetDocument("DA7BEAFBBD47CCD3C12580340044EAD2");
                                if (docObj != null && docObj.IsInitialized) {
                                    TestContext.WriteLine("DocumentObject Found");
                                    TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                    TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                    TestContext.WriteLine("Form: " + docObj.Form);
                                    TestContext.WriteLine("URL: " + docObj.Url);
                                    Assert.AreEqual(true, docObj.IsInitialized);
                                } else {
                                    //we have an error - check Connector.ReturnMessages
                                    Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void AllDocumentRetrievalTestingByUnids() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the documents by providing the unid - this function will call documentobject initialize and the isInitialized flag is set
                                if (dbObj.GetAllDocumentsByUnids("2C5911AF376C1CC0C1257EBC00551BD9;612E2B210D929B4CC1257EBC00574D3B;BC86569D21327D59C1257EDD003E22A0;08AEFE340CCFE152C1258199002F93E8") && !Connector.HasError) {
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                            } else {
                                                //we have an error - check Connector.ReturnMessages
                                                Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }
                                        }
                                        TestContext.WriteLine("Documents Requested 4 - Documents Received " + dbObj.Documents.Count.ToString());
                                        Assert.AreEqual(4, dbObj.Documents.Count);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                                if (docObj != null && docObj.IsInitialized) {
                                    TestContext.WriteLine("DocumentObject Found");
                                    TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                    TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                    TestContext.WriteLine("Form: " + docObj.Form);
                                    TestContext.WriteLine("URL: " + docObj.Url);
                                    Assert.AreEqual(true, docObj.IsInitialized);
                                } else {
                                    //we have an error - check Connector.ReturnMessages
                                    Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void AllDocumentRetrievalTestingByUnidsLoadAllFields() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the documents by providing the unid - this function will call documentobject initialize and the isInitialized flag is set
                                if (dbObj.GetAllDocumentsByUnids("2C5911AF376C1CC0C1257EBC00551BD9;612E2B210D929B4CC1257EBC00574D3B;BC86569D21327D59C1257EDD003E22A0;08AEFE340CCFE152C1258199002F93E8") && !Connector.HasError) {
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                // get all the fields in this document - this function will load the fields property
                                                if (docObj.GetAllFields()) {
                                                    if (docObj.Fields.Count > 0) {
                                                        TestContext.WriteLine("DocumentObject Fields:");
                                                        foreach (KeyValuePair<string, FieldObject> kvpField in docObj.Fields) {
                                                            TestContext.WriteLine(kvpField.Key + " : " + kvpField.Value.GetValueAsString());
                                                        }
                                                    } else {
                                                        TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                                        Assert.Fail("DocumentObject NO Fields Found!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                    }
                                                } else {
                                                    Assert.Fail("DocumentObject All Fields NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                }
                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                        } // end for each doc
                                        TestContext.WriteLine("Documents Requested 4 - Documents Received " + dbObj.Documents.Count.ToString());
                                        Assert.AreEqual(4, dbObj.Documents.Count);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }


        [TestMethod]
        public void AllDocumentRetrievalTestingByUnidsLoadAllFieldsAndFiles() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the documents by providing the unid - this function will call documentobject initialize and the isInitialized flag is set
                                // besides the default document information the file objects will be retrieved as well - this will allow us later on to extract them from JPI
                                if (dbObj.GetAllDocumentsAndFilesByUnids("2C5911AF376C1CC0C1257EBC00551BD9;612E2B210D929B4CC1257EBC00574D3B;BC86569D21327D59C1257EDD003E22A0;08AEFE340CCFE152C1258199002F93E8") && !Connector.HasError) {
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                TestContext.WriteLine("");
                                                // get all the fields in this document - this function will load the fields property
                                                if (docObj.GetAllFields()) {
                                                    if (docObj.Fields.Count > 0) {
                                                        TestContext.WriteLine("DocumentObject Fields:");
                                                        foreach (KeyValuePair<string, FieldObject> kvpField in docObj.Fields) {
                                                            TestContext.WriteLine(kvpField.Key + " : " + kvpField.Value.GetValueAsString());
                                                        }
                                                    } else {
                                                        TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                                        Assert.Fail("DocumentObject NO Fields Found!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                    }
                                                } else {
                                                    Assert.Fail("DocumentObject All Fields NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                }

                                                // get files object info displayed
                                                if (docObj.Files != null && docObj.Files.Count > 0) {
                                                    // we have files - let's write this to the TestContext here
                                                    foreach (KeyValuePair<string, FileObject> kvpFile in docObj.Files) {
                                                        TestContext.WriteLine("");
                                                        TestContext.WriteLine("File Object Found: " + kvpFile.Key);
                                                        TestContext.WriteLine("Created On: " + kvpFile.Value.DateCreated.ToString());
                                                        TestContext.WriteLine("Modified On: " + kvpFile.Value.DateModified.ToString());
                                                        TestContext.WriteLine("Creator: " + kvpFile.Value.Creator);
                                                        TestContext.WriteLine("URL: " + kvpFile.Value.URL);
                                                    }
                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FILES FOUND");
                                                }

                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                            TestContext.WriteLine("");
                                        } // end for each doc
                                        TestContext.WriteLine("Documents Requested 4 - Documents Received " + dbObj.Documents.Count.ToString());
                                        Assert.AreEqual(4, dbObj.Documents.Count);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void AllDocumentRetrievalTestingByUnidsLoadAll() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the documents by providing the unid - this function will call documentobject initialize and the isInitialized flag is set
                                // besides the default document information the file objects will be retrieved as well - this will allow us later on to extract them from JPI
                                if (dbObj.GetAllDocumentsAndFilesAndFieldsByUnids("2C5911AF376C1CC0C1257EBC00551BD9;612E2B210D929B4CC1257EBC00574D3B;BC86569D21327D59C1257EDD003E22A0;08AEFE340CCFE152C1258199002F93E8") && !Connector.HasError) {
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                TestContext.WriteLine("");
                                                // get all the fields in this document - this function will load the fields property

                                                if (docObj.Fields.Count > 0) {
                                                    TestContext.WriteLine("DocumentObject Fields:");
                                                    foreach (KeyValuePair<string, FieldObject> kvpField in docObj.Fields) {
                                                        TestContext.WriteLine(kvpField.Key + " : " + kvpField.Value.GetValueAsString());
                                                    }
                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                                    Assert.Fail("DocumentObject NO Fields Found!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                }


                                                // get files object info displayed
                                                if (docObj.Files != null && docObj.Files.Count > 0) {
                                                    // we have files - let's write this to the TestContext here
                                                    foreach (KeyValuePair<string, FileObject> kvpFile in docObj.Files) {
                                                        TestContext.WriteLine("");
                                                        TestContext.WriteLine("File Object Found: " + kvpFile.Key);
                                                        TestContext.WriteLine("Created On: " + kvpFile.Value.DateCreated.ToString());
                                                        TestContext.WriteLine("Modified On: " + kvpFile.Value.DateModified.ToString());
                                                        TestContext.WriteLine("Creator: " + kvpFile.Value.Creator);
                                                        TestContext.WriteLine("URL: " + kvpFile.Value.URL);
                                                    }
                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FILES FOUND");
                                                }

                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                            TestContext.WriteLine("");
                                        } // end for each doc
                                        TestContext.WriteLine("Documents Requested 4 - Documents Received " + dbObj.Documents.Count.ToString());
                                        Assert.AreEqual(4, dbObj.Documents.Count);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void AllDocumentRetrievalTestingByUnidsLoadAllSpecific() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the documents by providing the unid - this function will call documentobject initialize and the isInitialized flag is set
                                // besides the default document information the file objects will be retrieved as well - this will allow us later on to extract them from JPI
                                if (dbObj.GetAllDocumentsAndFilesAndFieldsByUnids("2C5911AF376C1CC0C1257EBC00551BD9;612E2B210D929B4CC1257EBC00574D3B;BC86569D21327D59C1257EDD003E22A0;08AEFE340CCFE152C1258199002F93E8", fieldNameList) && !Connector.HasError) {
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                TestContext.WriteLine("");
                                                // get all the fields in this document - this function will load the fields property

                                                if (docObj.Fields.Count > 0) {
                                                    TestContext.WriteLine("DocumentObject Fields:");
                                                    foreach (KeyValuePair<string, FieldObject> kvpField in docObj.Fields) {
                                                        TestContext.WriteLine(kvpField.Key + " : " + kvpField.Value.GetValueAsString());
                                                    }
                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                                    Assert.Fail("DocumentObject NO Fields Found!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                }


                                                // get files object info displayed
                                                if (docObj.Files != null && docObj.Files.Count > 0) {
                                                    // we have files - let's write this to the TestContext here
                                                    foreach (KeyValuePair<string, FileObject> kvpFile in docObj.Files) {
                                                        TestContext.WriteLine("");
                                                        TestContext.WriteLine("File Object Found: " + kvpFile.Key);
                                                        TestContext.WriteLine("Created On: " + kvpFile.Value.DateCreated.ToString());
                                                        TestContext.WriteLine("Modified On: " + kvpFile.Value.DateModified.ToString());
                                                        TestContext.WriteLine("Creator: " + kvpFile.Value.Creator);
                                                        TestContext.WriteLine("URL: " + kvpFile.Value.URL);
                                                    }
                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FILES FOUND");
                                                }

                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                            TestContext.WriteLine("");
                                        } // end for each doc
                                        TestContext.WriteLine("Documents Requested 4 - Documents Received " + dbObj.Documents.Count.ToString());
                                        Assert.AreEqual(4, dbObj.Documents.Count);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }


        [TestMethod]
        public void AllDocumentRetrievalTestingByFieldValueLoadAllSpecific() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the documents by providing the field and value - this function will call documentobject initialize and the isInitialized flag is set
                                // besides the default document information the file objects will be retrieved as well - this will allow us later on to extract them from JPI
                                if (dbObj.GetAllDocumentsAndFilesAndFieldsByKey("Form","DocIP", fieldNameList) && !Connector.HasError) { // get's specific fields
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        TestContext.WriteLine("Documents Received " + dbObj.Documents.Count.ToString());
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                TestContext.WriteLine("");
                                                // get all the fields in this document - this function will load the fields property

                                                if (docObj.Fields.Count > 0) {
                                                    TestContext.WriteLine("DocumentObject Fields:");
                                                    foreach (KeyValuePair<string, FieldObject> kvpField in docObj.Fields) {
                                                        TestContext.WriteLine(kvpField.Key + " : " + kvpField.Value.GetValueAsString());
                                                    }
                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                                    Assert.Fail("DocumentObject NO Fields Found!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                }


                                                // get files object info displayed
                                                if (docObj.Files != null && docObj.Files.Count > 0) {
                                                    // we have files - let's write this to the TestContext here
                                                    foreach (KeyValuePair<string, FileObject> kvpFile in docObj.Files) {
                                                        TestContext.WriteLine("");
                                                        TestContext.WriteLine("File Object Found: " + kvpFile.Key);
                                                        TestContext.WriteLine("Created On: " + kvpFile.Value.DateCreated.ToString());
                                                        TestContext.WriteLine("Modified On: " + kvpFile.Value.DateModified.ToString());
                                                        TestContext.WriteLine("Creator: " + kvpFile.Value.Creator);
                                                        TestContext.WriteLine("URL: " + kvpFile.Value.URL);
                                                    }
                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FILES FOUND");
                                                }

                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                            TestContext.WriteLine("");
                                        } // end for each doc
                                          //  TestContext.WriteLine("Documents Received " + dbObj.Documents.Count.ToString());
                                        Assert.AreEqual(dbObj.Documents.Count > 0, true);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }


        [TestMethod]
        public void AllDocumentRetrievalTestingByFormulaLoadAllSpecific() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the documents by providing the unid - this function will call documentobject initialize and the isInitialized flag is set
                                // besides the default document information the file objects will be retrieved as well - this will allow us later on to extract them from JPI
                                if (dbObj.GetAllDocumentsAndFilesAndFieldsByFormula("Form = \"DocIP\"", fieldNameList) && !Connector.HasError) {
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        TestContext.WriteLine("Documents Received " + dbObj.Documents.Count.ToString());
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                TestContext.WriteLine("");
                                                // get all the fields in this document - this function will load the fields property

                                                if (docObj.Fields.Count > 0) {
                                                    TestContext.WriteLine("DocumentObject Fields:");
                                                    foreach (KeyValuePair<string, FieldObject> kvpField in docObj.Fields) {
                                                        TestContext.WriteLine(kvpField.Key + " : " + kvpField.Value.GetValueAsString());
                                                    }
                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                                    Assert.Fail("DocumentObject NO Fields Found!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                }


                                                // get files object info displayed
                                                if (docObj.Files != null && docObj.Files.Count > 0) {
                                                    // we have files - let's write this to the TestContext here
                                                    foreach (KeyValuePair<string, FileObject> kvpFile in docObj.Files) {
                                                        TestContext.WriteLine("");
                                                        TestContext.WriteLine("File Object Found: " + kvpFile.Key);
                                                        TestContext.WriteLine("Created On: " + kvpFile.Value.DateCreated.ToString());
                                                        TestContext.WriteLine("Modified On: " + kvpFile.Value.DateModified.ToString());
                                                        TestContext.WriteLine("Creator: " + kvpFile.Value.Creator);
                                                        TestContext.WriteLine("URL: " + kvpFile.Value.URL);
                                                    }
                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FILES FOUND");
                                                }

                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                            TestContext.WriteLine("");
                                        } // end for each doc
                                      //  TestContext.WriteLine("Documents Received " + dbObj.Documents.Count.ToString());
                                        Assert.AreEqual(dbObj.Documents.Count > 0, true);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void AllDocumentRetrievalTestingByFieldValueLoadAll() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the documents by providing the field and value - this function will call documentobject initialize and the isInitialized flag is set
                                // besides the default document information the file objects will be retrieved as well - this will allow us later on to extract them from JPI
                                if (dbObj.GetAllDocumentsAndFilesAndFieldsByKey("Form", "DocIP") && !Connector.HasError) {
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        TestContext.WriteLine("Documents Received " + dbObj.Documents.Count.ToString());
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                TestContext.WriteLine("");
                                                // get all the fields in this document - this function will load the fields property

                                                if (docObj.Fields.Count > 0) {
                                                    TestContext.WriteLine("DocumentObject Fields:");
                                                    foreach (KeyValuePair<string, FieldObject> kvpField in docObj.Fields) {
                                                        TestContext.WriteLine(kvpField.Key + " : " + kvpField.Value.GetValueAsString());
                                                    }
                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                                    Assert.Fail("DocumentObject NO Fields Found!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                }


                                                // get files object info displayed
                                                if (docObj.Files != null && docObj.Files.Count > 0) {
                                                    // we have files - let's write this to the TestContext here
                                                    foreach (KeyValuePair<string, FileObject> kvpFile in docObj.Files) {
                                                        TestContext.WriteLine("");
                                                        TestContext.WriteLine("File Object Found: " + kvpFile.Key);
                                                        TestContext.WriteLine("Created On: " + kvpFile.Value.DateCreated.ToString());
                                                        TestContext.WriteLine("Modified On: " + kvpFile.Value.DateModified.ToString());
                                                        TestContext.WriteLine("Creator: " + kvpFile.Value.Creator);
                                                        TestContext.WriteLine("URL: " + kvpFile.Value.URL);
                                                    }
                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FILES FOUND");
                                                }

                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                            TestContext.WriteLine("");
                                        } // end for each doc
                                          //  TestContext.WriteLine("Documents Received " + dbObj.Documents.Count.ToString());
                                        Assert.AreEqual(dbObj.Documents.Count > 0, true);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void AllDocumentRetrievalTestingByFormulaLoadAll() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the documents by providing a formula - this function will call documentobject initialize and the isInitialized flag is set
                                // besides the default document information the file objects will be retrieved as well - this will allow us later on to extract them from JPI
                                if (dbObj.GetAllDocumentsAndFilesAndFieldsByFormula("Form = \"DocIP\"") && !Connector.HasError) { //all fields will be retrieved
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        TestContext.WriteLine("Documents Received " + dbObj.Documents.Count.ToString());
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                TestContext.WriteLine("");
                                                // get all the fields in this document - this function will load the fields property

                                                if (docObj.Fields.Count > 0) {
                                                    TestContext.WriteLine("DocumentObject Fields:");
                                                    foreach (KeyValuePair<string, FieldObject> kvpField in docObj.Fields) {
                                                        TestContext.WriteLine(kvpField.Key + " : " + kvpField.Value.GetValueAsString());
                                                    }
                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                                    Assert.Fail("DocumentObject NO Fields Found!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                }


                                                // get files object info displayed
                                                if (docObj.Files != null && docObj.Files.Count > 0) {
                                                    // we have files - let's write this to the TestContext here
                                                    foreach (KeyValuePair<string, FileObject> kvpFile in docObj.Files) {
                                                        TestContext.WriteLine("");
                                                        TestContext.WriteLine("File Object Found: " + kvpFile.Key);
                                                        TestContext.WriteLine("Created On: " + kvpFile.Value.DateCreated.ToString());
                                                        TestContext.WriteLine("Modified On: " + kvpFile.Value.DateModified.ToString());
                                                        TestContext.WriteLine("Creator: " + kvpFile.Value.Creator);
                                                        TestContext.WriteLine("URL: " + kvpFile.Value.URL);
                                                    }
                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FILES FOUND");
                                                }

                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                            TestContext.WriteLine("");
                                        } // end for each doc
                                        TestContext.WriteLine("Documents Received " + dbObj.Documents.Count.ToString());
                                        Assert.AreEqual(dbObj.Documents.Count > 0, true);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }


        [TestMethod]
        public void AllDocumentRetrievalTestingByFormulaLoadAllDownload() {
            try {
                // variable to track the amount of downloaded files
                long totalFileSizeAllDocuments = 0;
                // variable to count amount of files
                int totalFileCount = 0;
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                TestContext.WriteLine("Retrieving Documents by Formula: Form = \"DocIP\"");
                                // get the documents by providing the unid - this function will call documentobject initialize and the isInitialized flag is set
                                // besides the default document information the file objects will be retrieved as well - this will allow us later on to extract them from JPI
                                System.Diagnostics.Stopwatch watchRetrievalDocuments = System.Diagnostics.Stopwatch.StartNew();

                                //add timer to check how long this function is taking
                                if (dbObj.GetAllDocumentsAndFilesAndFieldsByFormula("Form = \"DocIP\"") && !Connector.HasError) {
                                    watchRetrievalDocuments.Stop();
                                    TestContext.WriteLine("Retrieved Documents in : " + watchRetrievalDocuments.ElapsedMilliseconds.ToString() + " ms");
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        TestContext.WriteLine("Documents Found : " + dbObj.Documents.Count.ToString());
                                        System.Diagnostics.Stopwatch watchEntireDownload = System.Diagnostics.Stopwatch.StartNew();
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                TestContext.WriteLine("");

                                                if (docObj.Fields.Count > 0) {
                                                    TestContext.WriteLine("DocumentObject Fields:");
                                                    foreach (KeyValuePair<string, FieldObject> kvpField in docObj.Fields) {
                                                        TestContext.WriteLine(kvpField.Key + " : " + kvpField.Value.GetValueAsString());
                                                    }
                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                                    Assert.Fail("DocumentObject NO Fields Found!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                }


                                                TestContext.WriteLine("");

                                                // get files object info displayed
                                                if (docObj.Files != null && docObj.Files.Count > 0) {
                                                    // Extract all via document method

                                                    long totalFileSize = 0;
                                                    // we have files - let's write this to the TestContext here
                                                    foreach (KeyValuePair<string, FileObject> kvpFile in docObj.Files) {
                                                        TestContext.WriteLine("");
                                                        TestContext.WriteLine("File Object Found: " + kvpFile.Key + "  -  Size: " + kvpFile.Value.Size + " bytes");
                                                        totalFileSize = totalFileSize + kvpFile.Value.Size;
                                                    }
                                                    // add the total file sizes of this doc to our counter
                                                    totalFileSizeAllDocuments = totalFileSizeAllDocuments + totalFileSize;
                                                    //create a directory per doc
                                                    string folderPath = "C:\\Users\\Acketk\\source\\repos\\VSTS\\XPagesAPI\\TestResults\\Files\\" + docObj.UniversalID;
                                                    if (System.IO.Directory.Exists(folderPath)) {
                                                        System.IO.Directory.Delete(folderPath, true);
                                                        TestContext.WriteLine("Deleted : " + folderPath);
                                                        System.IO.Directory.CreateDirectory(folderPath);
                                                    } else {
                                                        System.IO.Directory.CreateDirectory(folderPath);
                                                    }
                                                    TestContext.WriteLine("Created : " + folderPath);
                                                    TestContext.WriteLine("Exporting Files To : " + folderPath);
                                                    TestContext.WriteLine("Exporting File(s) : " + docObj.Files.Count.ToString());
                                                    // add the file count
                                                    totalFileCount = totalFileCount + docObj.Files.Count;
                                                    TestContext.WriteLine("Exporting File(s) Total Size : " + totalFileSize.ToString() + " bytes");
                                                    System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

                                                    ExtractAllFiles(folderPath, docObj);
                                                    watch.Stop();
                                                    TestContext.WriteLine("Exporting File(s) Total Download Time : " + watch.ElapsedMilliseconds.ToString() + " ms");

                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FILES FOUND");
                                                }

                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                            TestContext.WriteLine("");
                                        } // end for each doc
                                        watchEntireDownload.Stop();

                                        TestContext.WriteLine("Total Downloaded Files  : " + totalFileCount.ToString());
                                        TestContext.WriteLine("Total Downloaded File Size : " + totalFileSizeAllDocuments.ToString() + " bytes");
                                        TestContext.WriteLine("Total Download Time (including creating/deleting folders/files if needed) : " + watchEntireDownload.ElapsedMilliseconds.ToString() + " ms");
                                        Assert.AreEqual(dbObj.Documents.Count > 0, true);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    watchRetrievalDocuments.Stop();
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void AllDocumentRetrievalTestingByFormulaLoadAllSpecificDownload() {
            try {
                // variable to track the amount of downloaded files
                long totalFileSizeAllDocuments = 0;
                // variable to count amount of files
                int totalFileCount = 0;
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                TestContext.WriteLine("Retrieving Documents by Formula: Form = \"DocIP\"");
                                // get the documents by providing the unid - this function will call documentobject initialize and the isInitialized flag is set
                                // besides the default document information the file objects will be retrieved as well - this will allow us later on to extract them from JPI
                                System.Diagnostics.Stopwatch watchRetrievalDocuments = System.Diagnostics.Stopwatch.StartNew();

                                //add timer to check how long this function is taking
                                if (dbObj.GetAllDocumentsAndFilesAndFieldsByFormula("Form = \"DocIP\"", fieldNameList) && !Connector.HasError) {
                                    watchRetrievalDocuments.Stop();
                                    TestContext.WriteLine("Retrieved Documents in : " + watchRetrievalDocuments.ElapsedMilliseconds.ToString() + " ms");
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        TestContext.WriteLine("Documents Found : " + dbObj.Documents.Count.ToString());
                                        System.Diagnostics.Stopwatch watchEntireDownload = System.Diagnostics.Stopwatch.StartNew();
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                TestContext.WriteLine("");

                                                if (docObj.Fields.Count > 0) {
                                                    TestContext.WriteLine("DocumentObject Fields:");
                                                    foreach (KeyValuePair<string, FieldObject> kvpField in docObj.Fields) {
                                                        TestContext.WriteLine(kvpField.Key + " : " + kvpField.Value.GetValueAsString());
                                                    }
                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                                    Assert.Fail("DocumentObject NO Fields Found!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                }


                                                TestContext.WriteLine("");

                                                // get files object info displayed
                                                if (docObj.Files != null && docObj.Files.Count > 0) {
                                                    // Extract all via document method

                                                    long totalFileSize = 0;
                                                    // we have files - let's write this to the TestContext here
                                                    foreach (KeyValuePair<string, FileObject> kvpFile in docObj.Files) {
                                                        TestContext.WriteLine("");
                                                        TestContext.WriteLine("File Object Found: " + kvpFile.Key + "  -  Size: " + kvpFile.Value.Size + " bytes");
                                                        totalFileSize = totalFileSize + kvpFile.Value.Size;
                                                    }
                                                    // add the total file sizes of this doc to our counter
                                                    totalFileSizeAllDocuments = totalFileSizeAllDocuments + totalFileSize;
                                                    //create a directory per doc
                                                    string folderPath = "C:\\Users\\Acketk\\source\\repos\\VSTS\\XPagesAPI\\TestResults\\Files\\" + docObj.UniversalID;
                                                    if (System.IO.Directory.Exists(folderPath)) {
                                                        System.IO.Directory.Delete(folderPath, true);
                                                        TestContext.WriteLine("Deleted : " + folderPath);
                                                        System.IO.Directory.CreateDirectory(folderPath);
                                                    } else {
                                                        System.IO.Directory.CreateDirectory(folderPath);
                                                    }
                                                    TestContext.WriteLine("Created : " + folderPath);
                                                    TestContext.WriteLine("Exporting Files To : " + folderPath);
                                                    TestContext.WriteLine("Exporting File(s) : " + docObj.Files.Count.ToString());
                                                    // add the file count
                                                    totalFileCount = totalFileCount + docObj.Files.Count;
                                                    TestContext.WriteLine("Exporting File(s) Total Size : " + totalFileSize.ToString() + " bytes");
                                                    System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

                                                    ExtractAllFiles(folderPath, docObj);
                                                    watch.Stop();
                                                    TestContext.WriteLine("Exporting File(s) Total Download Time : " + watch.ElapsedMilliseconds.ToString() + " ms");

                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FILES FOUND");
                                                }

                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                            TestContext.WriteLine("");
                                        } // end for each doc
                                        watchEntireDownload.Stop();

                                        TestContext.WriteLine("Total Downloaded Files  : " + totalFileCount.ToString());
                                        TestContext.WriteLine("Total Downloaded File Size : " + totalFileSizeAllDocuments.ToString() + " bytes");
                                        TestContext.WriteLine("Total Download Time (including creating/deleting folders/files if needed) : " + watchEntireDownload.ElapsedMilliseconds.ToString() + " ms");
                                        Assert.AreEqual(dbObj.Documents.Count > 0, true);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    watchRetrievalDocuments.Stop();
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void AllDocumentRetrievalTestingByUnidsLoadAllFieldsAndFilesAndDownload() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the documents by providing the unid - this function will call documentobject initialize and the isInitialized flag is set
                                // besides the default document information the file objects will be retrieved as well - this will allow us later on to extract them from JPI
                                if (dbObj.GetAllDocumentsAndFilesByUnids("2C5911AF376C1CC0C1257EBC00551BD9;612E2B210D929B4CC1257EBC00574D3B;BC86569D21327D59C1257EDD003E22A0;08AEFE340CCFE152C1258199002F93E8") && !Connector.HasError) {
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                TestContext.WriteLine("");
                                                // get all the fields in this document - this function will load the fields property
                                                if (docObj.GetAllFields()) {
                                                    if (docObj.Fields.Count > 0) {
                                                        TestContext.WriteLine("DocumentObject Fields:");
                                                        foreach (KeyValuePair<string, FieldObject> kvpField in docObj.Fields) {
                                                            TestContext.WriteLine(kvpField.Key + " : " + kvpField.Value.GetValueAsString());
                                                        }
                                                    } else {
                                                        TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                                        Assert.Fail("DocumentObject NO Fields Found!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                    }
                                                } else {
                                                    Assert.Fail("DocumentObject All Fields NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                }

                                                // get files object info displayed
                                                if (docObj.Files != null && docObj.Files.Count > 0) {
                                                    // we have files - let's write this to the TestContext here
                                                    foreach (KeyValuePair<string, FileObject> kvpFile in docObj.Files) {
                                                        TestContext.WriteLine("");
                                                        TestContext.WriteLine("File Object Found: " + kvpFile.Key);
                                                        TestContext.WriteLine("Created On: " + kvpFile.Value.DateCreated.ToString());
                                                        TestContext.WriteLine("Modified On: " + kvpFile.Value.DateModified.ToString());
                                                        TestContext.WriteLine("Creator: " + kvpFile.Value.Creator);
                                                        TestContext.WriteLine("URL: " + kvpFile.Value.URL);

                                                    }
                                                    ExtractAllFiles("C:\\Users\\Acketk\\source\\repos\\VSTS\\XPagesAPI\\TestResults\\Files", docObj);
                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FILES FOUND");
                                                }

                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                            TestContext.WriteLine("");
                                        } // end for each doc
                                        TestContext.WriteLine("Documents Requested 4 - Documents Received " + dbObj.Documents.Count.ToString());
                                        Assert.AreEqual(4, dbObj.Documents.Count);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void AllDocumentRetrievalTestingByUnidsLoadAllFilesAndDownload() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the documents by providing the unid - this function will call documentobject initialize and the isInitialized flag is set
                                // besides the default document information the file objects will be retrieved as well - this will allow us later on to extract them from JPI
                                if (dbObj.GetAllDocumentsAndFilesByUnids("2C5911AF376C1CC0C1257EBC00551BD9;612E2B210D929B4CC1257EBC00574D3B;BC86569D21327D59C1257EDD003E22A0;08AEFE340CCFE152C1258199002F93E8") && !Connector.HasError) {
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                TestContext.WriteLine("");

                                                // get files object info displayed
                                                if (docObj.Files != null && docObj.Files.Count > 0) {
                                                    // we have files - let's write this to the TestContext here
                                                    foreach (KeyValuePair<string, FileObject> kvpFile in docObj.Files) {
                                                        TestContext.WriteLine("");
                                                        TestContext.WriteLine("File Object Found: " + kvpFile.Key);
                                                        TestContext.WriteLine("Created On: " + kvpFile.Value.DateCreated.ToString());
                                                        TestContext.WriteLine("Modified On: " + kvpFile.Value.DateModified.ToString());
                                                        TestContext.WriteLine("Creator: " + kvpFile.Value.Creator);
                                                        TestContext.WriteLine("URL: " + kvpFile.Value.URL);

                                                    }
                                                    TestContext.WriteLine("");

                                                    ExtractAllFiles("C:\\Users\\Acketk\\source\\repos\\VSTS\\XPagesAPI\\TestResults\\Files", docObj);

                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FILES FOUND");
                                                }

                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                            TestContext.WriteLine("");
                                        } // end for each doc
                                        TestContext.WriteLine("Documents Requested 4 - Documents Received " + dbObj.Documents.Count.ToString());
                                        Assert.AreEqual(4, dbObj.Documents.Count);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void AllDocumentRetrievalTestingByFormulaLoadAllFilesAndExport() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the documents by providing the unid - this function will call documentobject initialize and the isInitialized flag is set
                                // besides the default document information the file objects will be retrieved as well - this will allow us later on to extract them from JPI
                                if (dbObj.GetAllDocumentsAndFilesByFormula("Form = \"DocIssued\"") && !Connector.HasError) {
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        TestContext.WriteLine("Documents Found : " + dbObj.Documents.Count.ToString());
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                TestContext.WriteLine("");

                                                // get files object info displayed
                                                if (docObj.Files != null && docObj.Files.Count > 0) {
                                                    // Extract all via document method

                                                    long totalFileSize = 0;
                                                    // we have files - let's write this to the TestContext here
                                                    foreach (KeyValuePair<string, FileObject> kvpFile in docObj.Files) {
                                                        TestContext.WriteLine("");
                                                        TestContext.WriteLine("File Object Found: " + kvpFile.Key + "  -  Size: " + kvpFile.Value.Size + " bytes");
                                                        totalFileSize = totalFileSize + kvpFile.Value.Size;
                                                    }

                                                    //create a directory per doc
                                                    string folderPath = "C:\\Users\\Acketk\\source\\repos\\VSTS\\XPagesAPI\\TestResults\\Files\\" + docObj.UniversalID;
                                                    if (System.IO.Directory.Exists(folderPath)) {
                                                        System.IO.Directory.Delete(folderPath, true);
                                                        TestContext.WriteLine("Deleted : " + folderPath);
                                                        System.IO.Directory.CreateDirectory(folderPath);
                                                    } else {
                                                        System.IO.Directory.CreateDirectory(folderPath);
                                                    }
                                                    TestContext.WriteLine("Created : " + folderPath);
                                                    TestContext.WriteLine("Exporting Files To : " + folderPath);
                                                    TestContext.WriteLine("Exporting File(s) : " + docObj.Files.Count.ToString());
                                                    TestContext.WriteLine("Exporting File(s) Total Size : " + totalFileSize.ToString() + " bytes");
                                                    System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

                                                    if (docObj.ExportFiles(folderPath, true)) {
                                                        watch.Stop();
                                                        TestContext.WriteLine("Exporting File(s) Total Download Time : " + watch.ElapsedMilliseconds.ToString() + " ms");
                                                    } else {
                                                        watch.Stop();
                                                        Assert.Fail("Unable to Export Files!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                    }


                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FILES FOUND");
                                                }

                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                            TestContext.WriteLine("");
                                        } // end for each doc
                                        TestContext.WriteLine("Documents Requested 4 - Documents Received " + dbObj.Documents.Count.ToString());
                                        Assert.AreEqual(dbObj.Documents.Count > 0, true);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void AllDocumentRetrievalTestingByFormulaLoadAllFilesAndDownload() {
            try {
                // variable to track the amount of downloaded files
                long totalFileSizeAllDocuments = 0;
                // variable to count amount of files
                int totalFileCount = 0;
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                TestContext.WriteLine("Retrieving Documents by Formula: Form = \"DocIP\"");
                                // get the documents by providing the unid - this function will call documentobject initialize and the isInitialized flag is set
                                // besides the default document information the file objects will be retrieved as well - this will allow us later on to extract them from JPI
                                System.Diagnostics.Stopwatch watchRetrievalDocuments = System.Diagnostics.Stopwatch.StartNew();

                                //add timer to check how long this function is taking
                                if (dbObj.GetAllDocumentsAndFilesByFormula("Form = \"DocIP\"") && !Connector.HasError) {
                                    watchRetrievalDocuments.Stop();
                                    TestContext.WriteLine("Retrieved Documents in : " + watchRetrievalDocuments.ElapsedMilliseconds.ToString() + " ms");
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        TestContext.WriteLine("Documents Found : " + dbObj.Documents.Count.ToString());
                                        System.Diagnostics.Stopwatch watchEntireDownload = System.Diagnostics.Stopwatch.StartNew();
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                TestContext.WriteLine("");

                                                // get files object info displayed
                                                if (docObj.Files != null && docObj.Files.Count > 0) {
                                                    // Extract all via document method

                                                    long totalFileSize = 0;
                                                    // we have files - let's write this to the TestContext here
                                                    foreach (KeyValuePair<string, FileObject> kvpFile in docObj.Files) {
                                                        TestContext.WriteLine("");
                                                        TestContext.WriteLine("File Object Found: " + kvpFile.Key + "  -  Size: " + kvpFile.Value.Size + " bytes");
                                                        totalFileSize = totalFileSize + kvpFile.Value.Size;
                                                    }
                                                    // add the total file sizes of this doc to our counter
                                                    totalFileSizeAllDocuments = totalFileSizeAllDocuments + totalFileSize;
                                                    //create a directory per doc
                                                    string folderPath = "C:\\Users\\Acketk\\source\\repos\\VSTS\\XPagesAPI\\TestResults\\Files\\" + docObj.UniversalID;
                                                    if (System.IO.Directory.Exists(folderPath)) {
                                                        System.IO.Directory.Delete(folderPath, true);
                                                        TestContext.WriteLine("Deleted : " + folderPath);
                                                        System.IO.Directory.CreateDirectory(folderPath);
                                                    } else {
                                                        System.IO.Directory.CreateDirectory(folderPath);
                                                    }
                                                    TestContext.WriteLine("Created : " + folderPath);
                                                    TestContext.WriteLine("Exporting Files To : " + folderPath);
                                                    TestContext.WriteLine("Exporting File(s) : " + docObj.Files.Count.ToString());
                                                    // add the file count
                                                    totalFileCount = totalFileCount + docObj.Files.Count;
                                                    TestContext.WriteLine("Exporting File(s) Total Size : " + totalFileSize.ToString() + " bytes");
                                                    System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

                                                    ExtractAllFiles(folderPath, docObj);
                                                    watch.Stop();
                                                    TestContext.WriteLine("Exporting File(s) Total Download Time : " + watch.ElapsedMilliseconds.ToString() + " ms");

                                                } else {
                                                    TestContext.WriteLine("DocumentObject Files: NO FILES FOUND");
                                                }

                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                            TestContext.WriteLine("");
                                        } // end for each doc
                                        watchEntireDownload.Stop();

                                        TestContext.WriteLine("Total Downloaded Files  : " + totalFileCount.ToString());
                                        TestContext.WriteLine("Total Downloaded File Size : " + totalFileSizeAllDocuments.ToString() + " bytes");
                                        TestContext.WriteLine("Total Download Time (including creating/deleting folders/files if needed) : " + watchEntireDownload.ElapsedMilliseconds.ToString() + " ms");
                                        Assert.AreEqual(dbObj.Documents.Count > 0, true);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    watchRetrievalDocuments.Stop();
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void AllDocumentRetrievalTestingByFormulaLoadAllFieldsAllFilesAndDownload() {
            try {
                // variable to track the amount of downloaded files
                long totalFileSizeAllDocuments = 0;
                // variable to count amount of files
                int totalFileCount = 0;
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                TestContext.WriteLine("Retrieving Documents by Formula: Form = \"DocIP\"");
                                // get the documents by providing the unid - this function will call documentobject initialize and the isInitialized flag is set
                                // besides the default document information the file objects will be retrieved as well - this will allow us later on to extract them from JPI
                                System.Diagnostics.Stopwatch watchRetrievalDocuments = System.Diagnostics.Stopwatch.StartNew();

                                //add timer to check how long this function is taking
                                if (dbObj.GetAllDocumentsAndFilesByFormula("Form = \"DocIP\"") && !Connector.HasError) {
                                    watchRetrievalDocuments.Stop();
                                    TestContext.WriteLine("Retrieved Documents in : " + watchRetrievalDocuments.ElapsedMilliseconds.ToString() + " ms");
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        TestContext.WriteLine("Documents Found : " + dbObj.Documents.Count.ToString());
                                        System.Diagnostics.Stopwatch watchEntireDownload = System.Diagnostics.Stopwatch.StartNew();
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                TestContext.WriteLine("");


                                                // get all the fields in this document - this function will load the fields property
                                                if (docObj.GetAllFields()) {
                                                    if (docObj.Fields.Count > 0) {
                                                        TestContext.WriteLine("DocumentObject Fields:");
                                                        foreach (KeyValuePair<string, FieldObject> kvpField in docObj.Fields) {
                                                            TestContext.WriteLine(kvpField.Key + " : " + kvpField.Value.GetValueAsString());
                                                        }
                                                    } else {
                                                        TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                                        Assert.Fail("DocumentObject NO Fields Found!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                    }
                                                } else {
                                                    Assert.Fail("DocumentObject All Fields NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                }
                                                TestContext.WriteLine("");

                                                // get files object info displayed
                                                if (docObj.Files != null && docObj.Files.Count > 0) {
                                                    // Extract all via document method

                                                    long totalFileSize = 0;
                                                    // we have files - let's write this to the TestContext here
                                                    foreach (KeyValuePair<string, FileObject> kvpFile in docObj.Files) {
                                                        TestContext.WriteLine("");
                                                        TestContext.WriteLine("File Object Found: " + kvpFile.Key + "  -  Size: " + kvpFile.Value.Size + " bytes");
                                                        totalFileSize = totalFileSize + kvpFile.Value.Size;
                                                    }
                                                    // add the total file sizes of this doc to our counter
                                                    totalFileSizeAllDocuments = totalFileSizeAllDocuments + totalFileSize;
                                                    //create a directory per doc
                                                    string folderPath = "C:\\Users\\Acketk\\source\\repos\\VSTS\\XPagesAPI\\TestResults\\Files\\" + docObj.UniversalID;
                                                    if (System.IO.Directory.Exists(folderPath)) {
                                                        System.IO.Directory.Delete(folderPath, true);
                                                        TestContext.WriteLine("Deleted : " + folderPath);
                                                        System.IO.Directory.CreateDirectory(folderPath);
                                                    } else {
                                                        System.IO.Directory.CreateDirectory(folderPath);
                                                    }
                                                    TestContext.WriteLine("Created : " + folderPath);
                                                    TestContext.WriteLine("Exporting Files To : " + folderPath);
                                                    TestContext.WriteLine("Exporting File(s) : " + docObj.Files.Count.ToString());
                                                    // add the file count
                                                    totalFileCount = totalFileCount + docObj.Files.Count;
                                                    TestContext.WriteLine("Exporting File(s) Total Size : " + totalFileSize.ToString() + " bytes");
                                                    System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

                                                    ExtractAllFiles(folderPath, docObj);
                                                    watch.Stop();
                                                    TestContext.WriteLine("Exporting File(s) Total Download Time : " + watch.ElapsedMilliseconds.ToString() + " ms");

                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FILES FOUND");
                                                }

                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                            TestContext.WriteLine("");
                                        } // end for each doc
                                        watchEntireDownload.Stop();

                                        TestContext.WriteLine("Total Downloaded Files  : " + totalFileCount.ToString());
                                        TestContext.WriteLine("Total Downloaded File Size : " + totalFileSizeAllDocuments.ToString() + " bytes");
                                        TestContext.WriteLine("Total Download Time (including creating/deleting folders/files if needed) : " + watchEntireDownload.ElapsedMilliseconds.ToString() + " ms");
                                        Assert.AreEqual(dbObj.Documents.Count > 0, true);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    watchRetrievalDocuments.Stop();
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void AllDocumentRetrievalTestingByFormulaLoadSpecificFieldsAllFilesAndDownload() {
            try {
                // variable to track the amount of downloaded files
                long totalFileSizeAllDocuments = 0;
                // variable to count amount of files
                int totalFileCount = 0;
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                TestContext.WriteLine("Retrieving Documents by Formula: Form = \"DocIP\"");
                                // get the documents by providing the unid - this function will call documentobject initialize and the isInitialized flag is set
                                // besides the default document information the file objects will be retrieved as well - this will allow us later on to extract them from JPI
                                System.Diagnostics.Stopwatch watchRetrievalDocuments = System.Diagnostics.Stopwatch.StartNew();

                                //add timer to check how long this function is taking
                                if (dbObj.GetAllDocumentsAndFilesByFormula("Form = \"DocIP\"") && !Connector.HasError) {
                                    watchRetrievalDocuments.Stop();
                                    TestContext.WriteLine("Retrieved Documents in : " + watchRetrievalDocuments.ElapsedMilliseconds.ToString() + " ms");
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        TestContext.WriteLine("Documents Found : " + dbObj.Documents.Count.ToString());
                                        TestContext.WriteLine("Retrieving following fields : " + fieldNameList);
                                        TestContext.WriteLine("");
                                        System.Diagnostics.Stopwatch watchEntireDownload = System.Diagnostics.Stopwatch.StartNew();
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                TestContext.WriteLine("");


                                                // get some specific fields from this document - this function will load the fields property
                                                if (docObj.GetFields(fieldNameList)) {
                                                    if (docObj.Fields.Count > 0) {
                                                        TestContext.WriteLine("DocumentObject Fields:");
                                                        foreach (KeyValuePair<string, FieldObject> kvpField in docObj.Fields) {
                                                            TestContext.WriteLine(kvpField.Key + " : " + kvpField.Value.GetValueAsString());
                                                        }
                                                    } else {
                                                        TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                                        Assert.Fail("DocumentObject NO Fields Found!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                    }
                                                } else {
                                                    Assert.Fail("DocumentObject All Fields NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                                }
                                                TestContext.WriteLine("");

                                                // get files object info displayed
                                                if (docObj.Files != null && docObj.Files.Count > 0) {
                                                    // Extract all via document method

                                                    long totalFileSize = 0;
                                                    // we have files - let's write this to the TestContext here
                                                    foreach (KeyValuePair<string, FileObject> kvpFile in docObj.Files) {
                                                        TestContext.WriteLine("");
                                                        TestContext.WriteLine("File Object Found: " + kvpFile.Key + "  -  Size: " + kvpFile.Value.Size + " bytes");
                                                        totalFileSize = totalFileSize + kvpFile.Value.Size;
                                                    }
                                                    // add the total file sizes of this doc to our counter
                                                    totalFileSizeAllDocuments = totalFileSizeAllDocuments + totalFileSize;
                                                    //create a directory per doc
                                                    string folderPath = "C:\\Users\\Acketk\\source\\repos\\VSTS\\XPagesAPI\\TestResults\\Files\\" + docObj.UniversalID;
                                                    if (System.IO.Directory.Exists(folderPath)) {
                                                        System.IO.Directory.Delete(folderPath, true);
                                                        TestContext.WriteLine("Deleted : " + folderPath);
                                                        System.IO.Directory.CreateDirectory(folderPath);
                                                    } else {
                                                        System.IO.Directory.CreateDirectory(folderPath);
                                                    }
                                                    TestContext.WriteLine("Created : " + folderPath);
                                                    TestContext.WriteLine("Exporting Files To : " + folderPath);
                                                    TestContext.WriteLine("Exporting File(s) : " + docObj.Files.Count.ToString());
                                                    // add the file count
                                                    totalFileCount = totalFileCount + docObj.Files.Count;
                                                    TestContext.WriteLine("Exporting File(s) Total Size : " + totalFileSize.ToString() + " bytes");
                                                    System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

                                                    ExtractAllFiles(folderPath, docObj);
                                                    watch.Stop();
                                                    TestContext.WriteLine("Exporting File(s) Total Download Time : " + watch.ElapsedMilliseconds.ToString() + " ms");

                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FILES FOUND");
                                                }

                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                            TestContext.WriteLine("");
                                        } // end for each doc
                                        watchEntireDownload.Stop();

                                        TestContext.WriteLine("Total Downloaded Files  : " + totalFileCount.ToString());
                                        TestContext.WriteLine("Total Downloaded File Size : " + totalFileSizeAllDocuments.ToString() + " bytes");
                                        TestContext.WriteLine("Total Download Time (including creating/deleting folders/files if needed) : " + watchEntireDownload.ElapsedMilliseconds.ToString() + " ms");
                                        Assert.AreEqual(dbObj.Documents.Count > 0, true);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    watchRetrievalDocuments.Stop();
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void AllDocumentRetrievalTestingByFieldValueLoadAllFilesAndDownload() {
            try {
                // variable to track the amount of downloaded files
                long totalFileSizeAllDocuments = 0;
                // variable to count amount of files
                int totalFileCount = 0;
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("FilePath: " + dbObj.FilePath + " Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                TestContext.WriteLine("");
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                TestContext.WriteLine("Retrieving Documents by Key/Value - Form , DocIP");
                                // get the documents by providing the unid - this function will call documentobject initialize and the isInitialized flag is set
                                // besides the default document information the file objects will be retrieved as well - this will allow us later on to extract them from JPI
                                System.Diagnostics.Stopwatch watchRetrievalDocuments = System.Diagnostics.Stopwatch.StartNew();

                                //add timer to check how long this function is taking
                                if (dbObj.GetAllDocumentsAndFilesByKey("Form", "DocIP") && !Connector.HasError) {
                                    watchRetrievalDocuments.Stop();
                                    TestContext.WriteLine("Retrieved Documents in : " + watchRetrievalDocuments.ElapsedMilliseconds.ToString() + " ms");
                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {
                                        TestContext.WriteLine("Documents Found : " + dbObj.Documents.Count.ToString());
                                        System.Diagnostics.Stopwatch watchEntireDownload = System.Diagnostics.Stopwatch.StartNew();
                                        foreach (KeyValuePair<string, DocumentObject> kvp in dbObj.Documents) {
                                            docObj = kvp.Value;
                                            if (docObj != null && docObj.IsInitialized) {
                                                TestContext.WriteLine("DocumentObject Found : " + docObj.UniversalID);
                                                TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                                TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                                TestContext.WriteLine("Form: " + docObj.Form);
                                                TestContext.WriteLine("URL: " + docObj.Url);
                                                TestContext.WriteLine("");

                                                // get files object info displayed
                                                if (docObj.Files != null && docObj.Files.Count > 0) {
                                                    // Extract all via document method

                                                    long totalFileSize = 0;
                                                    // we have files - let's write this to the TestContext here
                                                    foreach (KeyValuePair<string, FileObject> kvpFile in docObj.Files) {
                                                        TestContext.WriteLine("");
                                                        TestContext.WriteLine("File Object Found: " + kvpFile.Key + "  -  Size: " + kvpFile.Value.Size + " bytes");
                                                        totalFileSize = totalFileSize + kvpFile.Value.Size;
                                                    }
                                                    // add the total file sizes of this doc to our counter
                                                    totalFileSizeAllDocuments = totalFileSizeAllDocuments + totalFileSize;
                                                    //create a directory per doc
                                                    string folderPath = "C:\\Users\\Acketk\\source\\repos\\VSTS\\XPagesAPI\\TestResults\\Files\\" + docObj.UniversalID;
                                                    if (System.IO.Directory.Exists(folderPath)) {
                                                        System.IO.Directory.Delete(folderPath, true);
                                                        TestContext.WriteLine("Deleted : " + folderPath);
                                                        System.IO.Directory.CreateDirectory(folderPath);
                                                    } else {
                                                        System.IO.Directory.CreateDirectory(folderPath);
                                                    }
                                                    TestContext.WriteLine("Created : " + folderPath);
                                                    TestContext.WriteLine("Exporting Files To : " + folderPath);
                                                    TestContext.WriteLine("Exporting File(s) : " + docObj.Files.Count.ToString());
                                                    // add the file count
                                                    totalFileCount = totalFileCount + docObj.Files.Count;
                                                    TestContext.WriteLine("Exporting File(s) Total Size : " + totalFileSize.ToString() + " bytes");
                                                    System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

                                                    ExtractAllFiles(folderPath, docObj);
                                                    watch.Stop();
                                                    TestContext.WriteLine("Exporting File(s) Total Download Time : " + watch.ElapsedMilliseconds.ToString() + " ms");

                                                } else {
                                                    TestContext.WriteLine("DocumentObject Fields: NO FILES FOUND");
                                                }

                                            } else {
                                                Assert.Fail("DocumentObject Not Initialized" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                            }

                                            TestContext.WriteLine("");
                                        } // end for each doc
                                        watchEntireDownload.Stop();

                                        TestContext.WriteLine("Total Downloaded Files  : " + totalFileCount.ToString());
                                        TestContext.WriteLine("Total Downloaded File Size : " + totalFileSizeAllDocuments.ToString() + " bytes");
                                        TestContext.WriteLine("Total Download Time (including creating/deleting folders/files if needed) : " + watchEntireDownload.ElapsedMilliseconds.ToString() + " ms");
                                        Assert.AreEqual(dbObj.Documents.Count > 0, true);

                                    } else {
                                        Assert.Fail("DatabaseObject Retrieved - None Found" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    watchRetrievalDocuments.Stop();
                                    Assert.Fail("DocumentObjects NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }

                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void DocumentRetrievalTestingByFormula() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the document by providing a formula - the first document found will be returned
                                //this function will call documentobject initialize and the isInitialized flag is set
                                docObj = dbObj.GetDocumentByFormula("INTERFACEKEY =\"ABCD00095\"");
                                if (docObj != null && docObj.IsInitialized) {
                                    TestContext.WriteLine("DocumentObject Found");
                                    TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                    TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                    TestContext.WriteLine("Form: " + docObj.Form);
                                    TestContext.WriteLine("URL: " + docObj.Url);
                                    Assert.AreEqual(true, docObj.IsInitialized);
                                } else {
                                    //we have an error - check Connector.ReturnMessages
                                    Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void DocumentRetrievalTestingByFieldValue() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the document by providing a search field and value - the first document found will be returned
                                //this function will call documentobject initialize and the isInitialized flag is set
                                docObj = dbObj.GetDocumentByKey("INTERFACEKEY", "ABCD00096");
                                if (docObj != null && docObj.IsInitialized) {
                                    TestContext.WriteLine("DocumentObject Found");
                                    TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                    TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                    TestContext.WriteLine("Form: " + docObj.Form);
                                    TestContext.WriteLine("URL: " + docObj.Url);
                                    Assert.AreEqual(true, docObj.IsInitialized);
                                } else {
                                    //we have an error - check Connector.ReturnMessages
                                    Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void DocumentRetrievalTestingByUnidLoadAllFields() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the document by providing a search field and value - the first document found will be returned
                                //this function will call documentobject initialize and the isInitialized flag is set
                                docObj = dbObj.GetDocument("DA7BEAFBBD47CCD3C12580340044EAD2");
                                if (docObj != null && docObj.IsInitialized) {
                                    TestContext.WriteLine("DocumentObject Found");
                                    TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                    TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                    TestContext.WriteLine("Form: " + docObj.Form);
                                    TestContext.WriteLine("URL: " + docObj.Url);
                                    // get all the fields in this document - this function will load the fields property
                                    if (docObj.GetAllFields()) {
                                        if (docObj.Fields.Count > 0) {
                                            TestContext.WriteLine("DocumentObject Fields:");
                                            foreach (KeyValuePair<string, FieldObject> kvp in docObj.Fields) {
                                                TestContext.WriteLine(kvp.Key + " : " + kvp.Value.GetValueAsString());
                                            }
                                        } else {
                                            TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                        }
                                        Assert.AreEqual(true, docObj.Fields != null);
                                    } else {
                                        Assert.Fail("DocumentObject All Fields NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    //we have an error - check Connector.ReturnMessages
                                    Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void DocumentRetrievalTestingByFormulaLoadFilesAndFields() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the document by providing a universal id of the document
                                //this function will call documentobject initialize and the isInitialized flag is set
                                docObj = dbObj.GetDocumentAndFilesAndFieldsByFormula("INTERFACEKEY = \"ABCD00095\"");
                                if (docObj != null && docObj.IsInitialized) {
                                    TestContext.WriteLine("DocumentObject Found");
                                    TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                    TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                    TestContext.WriteLine("Form: " + docObj.Form);
                                    TestContext.WriteLine("URL: " + docObj.Url);
                                    // file objects should already be retrieved see above GetDocumentAndFiles
                                    // if you use GetDocument - you can afterward use the docObj.GetFiles() method
                                    //- note this is slower - because it will get the document again
                                    if (docObj.Files.Count > 0) {
                                        TestContext.WriteLine("DocumentObject Files:");
                                        foreach (KeyValuePair<string, FileObject> kvp in docObj.Files) {
                                            TestContext.WriteLine(kvp.Key + " : " + kvp.Value.URL);
                                        }
                                    } else {
                                        // no files found
                                        TestContext.WriteLine("DocumentObject Files: NO FILES FOUND");
                                    }

                                    if (docObj.Fields.Count > 0) {
                                        TestContext.WriteLine("DocumentObject Fields:");
                                        foreach (KeyValuePair<string, FieldObject> kvpFields in docObj.Fields) {
                                            TestContext.WriteLine(kvpFields.Key + " : " + kvpFields.Value.GetValueAsString());
                                        }
                                    } else {
                                        TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                    }
                                    Assert.AreEqual(true, docObj.Fields != null);

                                    // Assert.AreEqual(true, docObj.Files != null);
                                } else {
                                    //we have an error - check Connector.ReturnMessages
                                    Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void DocumentRetrievalTestingByUnidLoadFilesAndFields() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the document by providing a universal id of the document
                                //this function will call documentobject initialize and the isInitialized flag is set
                                docObj = dbObj.GetDocumentAndFilesAndFields("BC86569D21327D59C1257EDD003E22A0");
                                if (docObj != null && docObj.IsInitialized) {
                                    TestContext.WriteLine("DocumentObject Found");
                                    TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                    TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                    TestContext.WriteLine("Form: " + docObj.Form);
                                    TestContext.WriteLine("URL: " + docObj.Url);
                                    // file objects should already be retrieved see above GetDocumentAndFiles
                                    // if you use GetDocument - you can afterward use the docObj.GetFiles() method
                                    //- note this is slower - because it will get the document again
                                    if (docObj.Files.Count > 0) {
                                        TestContext.WriteLine("DocumentObject Files:");
                                        foreach (KeyValuePair<string, FileObject> kvp in docObj.Files) {
                                            TestContext.WriteLine(kvp.Key + " : " + kvp.Value.URL);
                                        }
                                    } else {
                                        // no files found
                                        TestContext.WriteLine("DocumentObject Files: NO FILES FOUND");
                                    }

                                    if (docObj.Fields.Count > 0) {
                                        TestContext.WriteLine("DocumentObject Fields:");
                                        foreach (KeyValuePair<string, FieldObject> kvpFields in docObj.Fields) {
                                            TestContext.WriteLine(kvpFields.Key + " : " + kvpFields.Value.GetValueAsString());
                                        }
                                    } else {
                                        TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                    }
                                    Assert.AreEqual(true, docObj.Fields != null);

                                    // Assert.AreEqual(true, docObj.Files != null);
                                } else {
                                    //we have an error - check Connector.ReturnMessages
                                    Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }


        [TestMethod]
        public void DocumentRetrievalTestingByFieldValueLoadFilesAndFields() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the document by providing a universal id of the document
                                //this function will call documentobject initialize and the isInitialized flag is set
                                docObj = dbObj.GetDocumentAndFilesAndFieldsByKey("INTERFACEKEY", "ABCD00096");
                                if (docObj != null && docObj.IsInitialized) {
                                    TestContext.WriteLine("DocumentObject Found");
                                    TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                    TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                    TestContext.WriteLine("Form: " + docObj.Form);
                                    TestContext.WriteLine("URL: " + docObj.Url);
                                    // file objects should already be retrieved see above GetDocumentAndFiles
                                    // if you use GetDocument - you can afterward use the docObj.GetFiles() method
                                    //- note this is slower - because it will get the document again
                                    if (docObj.Files.Count > 0) {
                                        TestContext.WriteLine("DocumentObject Files:");
                                        foreach (KeyValuePair<string, FileObject> kvp in docObj.Files) {
                                            TestContext.WriteLine(kvp.Key + " : " + kvp.Value.URL);
                                        }
                                    } else {
                                        // no files found
                                        TestContext.WriteLine("DocumentObject Files: NO FILES FOUND");
                                    }

                                    if (docObj.Fields.Count > 0) {
                                        TestContext.WriteLine("DocumentObject Fields:");
                                        foreach (KeyValuePair<string, FieldObject> kvpFields in docObj.Fields) {
                                            TestContext.WriteLine(kvpFields.Key + " : " + kvpFields.Value.GetValueAsString());
                                        }
                                    } else {
                                        TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                    }
                                    Assert.AreEqual(true, docObj.Fields != null);

                                    // Assert.AreEqual(true, docObj.Files != null);
                                } else {
                                    //we have an error - check Connector.ReturnMessages
                                    Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void DocumentRetrievalTestingByFormulaLoadFiles() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the document by providing a universal id of the document
                                //this function will call documentobject initialize and the isInitialized flag is set
                                docObj = dbObj.GetDocumentAndFilesByFormula("INTERFACEKEY = \"ABCD00095\"");
                                if (docObj != null && docObj.IsInitialized) {
                                    TestContext.WriteLine("DocumentObject Found");
                                    TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                    TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                    TestContext.WriteLine("Form: " + docObj.Form);
                                    TestContext.WriteLine("URL: " + docObj.Url);
                                    // file objects should already be retrieved see above GetDocumentAndFiles
                                    // if you use GetDocument - you can afterward use the docObj.GetFiles() method
                                    //- note this is slower - because it will get the document again
                                    if (docObj.Files.Count > 0) {
                                        TestContext.WriteLine("DocumentObject Files:");
                                        foreach (KeyValuePair<string, FileObject> kvp in docObj.Files) {
                                            TestContext.WriteLine(kvp.Key + " : " + kvp.Value.URL);
                                        }
                                    } else {
                                        // no files found
                                        TestContext.WriteLine("DocumentObject Files: NO FILES FOUND");
                                    }

                                    Assert.AreEqual(true, docObj.Files != null);
                                } else {
                                    //we have an error - check Connector.ReturnMessages
                                    Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void DocumentRetrievalTestingByUnidLoadFiles() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the document by providing a universal id of the document
                                //this function will call documentobject initialize and the isInitialized flag is set
                                docObj = dbObj.GetDocumentAndFiles("DA7BEAFBBD47CCD3C12580340044EAD2");
                                if (docObj != null && docObj.IsInitialized) {
                                    TestContext.WriteLine("DocumentObject Found");
                                    TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                    TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                    TestContext.WriteLine("Form: " + docObj.Form);
                                    TestContext.WriteLine("URL: " + docObj.Url);
                                    // file objects should already be retrieved see above GetDocumentAndFiles
                                    // if you use GetDocument - you can afterward use the docObj.GetFiles() method
                                    //- note this is slower - because it will get the document again
                                    if (docObj.Files.Count > 0) {
                                        TestContext.WriteLine("DocumentObject Files:");
                                        foreach (KeyValuePair<string, FileObject> kvp in docObj.Files) {
                                            TestContext.WriteLine(kvp.Key + " : " + kvp.Value.URL);
                                        }
                                    } else {
                                        // no files found
                                        TestContext.WriteLine("DocumentObject Files: NO FILES FOUND");
                                    }

                                    Assert.AreEqual(true, docObj.Files != null);
                                } else {
                                    //we have an error - check Connector.ReturnMessages
                                    Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void DocumentRetrievalTestingByFieldValueLoadAllFields() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the document by providing a search field and value - the first document found will be returned
                                //this function will call documentobject initialize and the isInitialized flag is set
                                docObj = dbObj.GetDocumentByKey("INTERFACEKEY", "ABCD00096");
                                if (docObj != null && docObj.IsInitialized) {
                                    TestContext.WriteLine("DocumentObject Found");
                                    TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                    TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                    TestContext.WriteLine("Form: " + docObj.Form);
                                    TestContext.WriteLine("URL: " + docObj.Url);
                                    // get all the fields in this document - this function will load the fields property
                                    if (docObj.GetAllFields()) {
                                        if (docObj.Fields.Count > 0) {
                                            TestContext.WriteLine("DocumentObject Fields:");
                                            foreach (KeyValuePair<string, FieldObject> kvp in docObj.Fields) {
                                                TestContext.WriteLine(kvp.Key + " : " + kvp.Value.GetValueAsString());
                                            }
                                        } else {
                                            TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                        }
                                        Assert.AreEqual(true, docObj.Fields != null);
                                    } else {
                                        Assert.Fail("DocumentObject All Fields NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    //we have an error - check Connector.ReturnMessages
                                    Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void DocumentRetrievalTestingByFormulaLoadAllFields() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the document by providing a search field and value - the first document found will be returned
                                //this function will call documentobject initialize and the isInitialized flag is set
                                docObj = dbObj.GetDocumentByFormula("INTERFACEKEY = \"ABCD00095\"");
                                if (docObj != null && docObj.IsInitialized) {
                                    TestContext.WriteLine("DocumentObject Found");
                                    TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                    TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                    TestContext.WriteLine("Form: " + docObj.Form);
                                    TestContext.WriteLine("URL: " + docObj.Url);
                                    // get all the fields in this document - this function will load the fields property
                                    if (docObj.GetAllFields()) {
                                        if (docObj.Fields.Count > 0) {
                                            TestContext.WriteLine("DocumentObject Fields:");
                                            foreach (KeyValuePair<string, FieldObject> kvp in docObj.Fields) {
                                                TestContext.WriteLine(kvp.Key + " : " + kvp.Value.GetValueAsString());
                                            }
                                        } else {
                                            TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                        }
                                        Assert.AreEqual(true, docObj.Fields != null);
                                    } else {
                                        Assert.Fail("DocumentObject All Fields NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    //we have an error - check Connector.ReturnMessages
                                    Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void DocumentRetrievalTestingByFieldValueLoadFiles() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the document by providing a universal id of the document
                                //this function will call documentobject initialize and the isInitialized flag is set
                                docObj = dbObj.GetDocumentAndFilesByKey("INTERFACEKEY", "ABCD00095");
                                if (docObj != null && docObj.IsInitialized) {
                                    TestContext.WriteLine("DocumentObject Found");
                                    TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                    TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                    TestContext.WriteLine("Form: " + docObj.Form);
                                    TestContext.WriteLine("URL: " + docObj.Url);
                                    // file objects should already be retrieved see above GetDocumentAndFiles
                                    // if you use GetDocument - you can afterward use the docObj.GetFiles() method
                                    //- note this is slower - because it will get the document again
                                    if (docObj.Files.Count > 0) {
                                        TestContext.WriteLine("DocumentObject Files:");
                                        foreach (KeyValuePair<string, FileObject> kvp in docObj.Files) {
                                            TestContext.WriteLine(kvp.Key + " : " + kvp.Value.URL);
                                        }
                                    } else {
                                        // no files found
                                        TestContext.WriteLine("DocumentObject Files: NO FILES FOUND");
                                    }

                                    Assert.AreEqual(true, docObj.Files != null);
                                } else {
                                    //we have an error - check Connector.ReturnMessages
                                    Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void DocumentRetrievalTestingByFieldValueLoadSpecificFields() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the document by providing a search field and value - the first document found will be returned
                                //this function will call documentobject initialize and the isInitialized flag is set
                                docObj = dbObj.GetDocumentByKey("INTERFACEKEY", "ABCD00096");
                                if (docObj != null && docObj.IsInitialized) {
                                    TestContext.WriteLine("DocumentObject Found");
                                    TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                    TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                    TestContext.WriteLine("Form: " + docObj.Form);
                                    TestContext.WriteLine("URL: " + docObj.Url);
                                    // get some specific fields from the domino document
                                    if (docObj.GetFields("jeNumber;jeTitle1")) {
                                        if (docObj.Fields.Count > 0) {
                                            TestContext.WriteLine("DocumentObject Fields:");
                                            foreach (KeyValuePair<string, FieldObject> kvp in docObj.Fields) {
                                                TestContext.WriteLine(kvp.Key + " : " + kvp.Value.GetValueAsString());
                                            }
                                        } else {
                                            TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                        }
                                        Assert.AreEqual(true, docObj.Fields != null);
                                    } else {
                                        Assert.Fail("DocumentObject Specific Fields NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    //we have an error - check Connector.ReturnMessages
                                    Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void DocumentRetrievalTestingByUnidLoadSpecificFields() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the document by providing a search field and value - the first document found will be returned
                                //this function will call documentobject initialize and the isInitialized flag is set
                                docObj = dbObj.GetDocument("DA7BEAFBBD47CCD3C12580340044EAD2");
                                if (docObj != null && docObj.IsInitialized) {
                                    TestContext.WriteLine("DocumentObject Found");
                                    TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                    TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                    TestContext.WriteLine("Form: " + docObj.Form);
                                    TestContext.WriteLine("URL: " + docObj.Url);
                                    // get some specific fields from the domino document
                                    // get some specific fields from the domino document
                                    if (docObj.GetFields("jeNumber;jeTitle1")) {
                                        if (docObj.Fields.Count > 0) {
                                            TestContext.WriteLine("DocumentObject Fields:");
                                            foreach (KeyValuePair<string, FieldObject> kvp in docObj.Fields) {
                                                TestContext.WriteLine(kvp.Key + " : " + kvp.Value.GetValueAsString());
                                            }
                                        } else {
                                            TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                        }
                                        Assert.AreEqual(true, docObj.Fields != null);
                                    } else {
                                        Assert.Fail("DocumentObject Specific Fields NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    //we have an error - check Connector.ReturnMessages
                                    Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        [TestMethod]
        public void DocumentRetrievalTestingByFormulaLoadSpecificFields() {
            try {
                // to start the session we need to setup the connector
                Connector connector = null;
                // this is done by providing the user name, password and the domino server URL
                connector = new Connector(UserName, Password, ServerURL);
                // in the initialize function you can specify the encryption pass, iv and salt
                // Do not provide this unless you also change the Xpages Interface Database JPIService to reflect the new encryption strings

                //must always be called - if an invalid URL is provide this will return false
                if (connector.Initialize()) {
                    // Connect to the JPI IBM Domino Server - this function will execute an authentication request - sets flag isConnected
                    if (connector.Connect()) {
                        // create a new session to JPI via the XPages Interface Database - JPIService
                        SessionObject session = null;
                        // Get the session via our connector by providing the full URL to the JPIService in the XPages database
                        session = connector.GetSession(XPagesServiceURL);
                        if (Connector.HasError || session == null) {
                            //we have an error in creating/getting the session object
                            //  TestContext.WriteLine("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            Assert.Fail("Unable to Create a Session to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        } else {
                            // Retrieve a Database via the session object - provide the server & database filepath to connect to
                            DatabaseObject dbObj = null;
                            dbObj = new DatabaseObject(DatabaseFilePath, DominoServerName, session);
                            // Call initialize to establish a connection to the given database - if you have the access and the database can be found
                            if (dbObj.Initialize()) {
                                // Database found - db Isinitialized is set to true
                                TestContext.WriteLine("DatabaseObject Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                TestContext.WriteLine("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                // we have the database object - load documentobject
                                DocumentObject docObj = null;
                                // get the document by providing a search field and value - the first document found will be returned
                                //this function will call documentobject initialize and the isInitialized flag is set
                                docObj = dbObj.GetDocumentByFormula("INTERFACEKEY = \"ABCD00095\"");
                                if (docObj != null && docObj.IsInitialized) {
                                    TestContext.WriteLine("DocumentObject Found");
                                    TestContext.WriteLine("Created On: " + docObj.DateCreated);
                                    TestContext.WriteLine("Modified On: " + docObj.DateModified);
                                    TestContext.WriteLine("Form: " + docObj.Form);
                                    TestContext.WriteLine("URL: " + docObj.Url);
                                    // get some specific fields from the domino document
                                    // get some specific fields from the domino document
                                    if (docObj.GetFields("jeNumber;jeTitle1")) {
                                        if (docObj.Fields.Count > 0) {
                                            TestContext.WriteLine("DocumentObject Fields:");
                                            foreach (KeyValuePair<string, FieldObject> kvp in docObj.Fields) {
                                                TestContext.WriteLine(kvp.Key + " : " + kvp.Value.GetValueAsString());
                                            }
                                        } else {
                                            TestContext.WriteLine("DocumentObject Fields: NO FIELDS FOUND");
                                        }
                                        Assert.AreEqual(true, docObj.Fields != null);
                                    } else {
                                        Assert.Fail("DocumentObject Specific Fields NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                    }
                                } else {
                                    //we have an error - check Connector.ReturnMessages
                                    Assert.Fail("DocumentObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                }
                                // Assert.AreEqual(true, dbObj.IsInitialized);
                            } else {
                                // TestContext.WriteLine("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                                Assert.Fail("DatabaseObject NOT Retrieved!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                            }
                        }
                    } else {
                        //TestContext.WriteLine("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                        Assert.Fail("Unable to Connect to the JPI Domino Server" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    }
                } else {
                    //TestContext.WriteLine("Unable to Initialize the Connector!" +Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                    Assert.Fail("Unable to Initialize the Connector!" + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));
                }
            } catch (Exception ex) {
                //TestContext.WriteLine("Error : " + GetErrorInfo(ex));
                Assert.Fail("Error : " + GetErrorInfo(ex));
            }
        }

        private string GetReturnMessages(ArrayList arList) {
            if (arList != null && arList.Count > 0) {
                return string.Join(Environment.NewLine, arList.ToArray());
            } else {
                return "";
            }
        }

        private string GetErrorInfo(Exception ex) {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(ex, true);
            string FileName = "";
            string Method = "";
            string LineNumber = "";
            sb.AppendLine("");
            sb.AppendLine(ex.Message);
            sb.AppendLine("");
            foreach (System.Diagnostics.StackFrame frame in st.GetFrames()) {
                FileName = System.IO.Path.GetFileName(frame.GetFileName());
                Method = frame.GetMethod().ToString();
                LineNumber = frame.GetFileLineNumber().ToString();
                if (FileName != "")
                    sb.AppendLine("Filename : " + FileName);
                if (Method != "")
                    sb.AppendLine("Method : " + Method);
                if (LineNumber != "")
                    sb.AppendLine("Line N° : " + LineNumber);
            }
            return sb.ToString();
        }


        private void ExtractAllFiles(string rootFolder, DocumentObject docObj) {
            if (!string.IsNullOrEmpty(rootFolder) && docObj != null && docObj.IsInitialized && docObj.Files != null && docObj.Files.Count > 0) {
                foreach (KeyValuePair<string, FileObject> kvp in docObj.Files) {
                    //Extract each file
                    TestContext.WriteLine("File : " + kvp.Value.Name + " - Size : " + kvp.Value.Size + " bytes");
                    System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

                    if (kvp.Value.ExtractFile(rootFolder + "\\" + kvp.Value.Name) && !Connector.HasError) {
                        watch.Stop();

                        TestContext.WriteLine("Extracted To : " + rootFolder + "\\" + kvp.Value.Name);

                        long elapsedMs = watch.ElapsedMilliseconds;
                        TestContext.WriteLine("Time : " + elapsedMs.ToString() + " ms");
                    } else {
                        Assert.Fail("File NOT Retrieved : " + kvp.Value.Name + Environment.NewLine + "ReturnMessages : " + GetReturnMessages(Connector.ReturnMessages));

                    }
                }
            }

        }
    }
}