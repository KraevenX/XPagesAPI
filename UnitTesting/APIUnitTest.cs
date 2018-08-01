using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        // private const string DominoServerName = "JPI2/JPI";

        /// <summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get { return testContextInstance; }
            set { testContextInstance = value; }
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
    }
}