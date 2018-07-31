using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestDLL {
    public partial class Form2 : Form {
        public Form2() {
            InitializeComponent();
        }

        Connector connector;


        public string GetArrayListAsString(ArrayList arList, string separator) {
            string returnString = "";
            if (arList != null && arList.Count > 0) {
                foreach (string str in arList) {
                    returnString = returnString + str + separator;
                }
                returnString = returnString.Substring(0, returnString.LastIndexOf(separator));
            }
            return returnString;
        }

        private void btnConnect_Click(object sender, System.EventArgs e) {
            SessionObject s = null;
            DatabaseObject dbObj = null;
            DocumentObject docObj = null;
            try {
                if (txtUser.Text != null && txtPass.Text != null && txtServer.Text != null) {
                    connector = new Connector(txtUser.Text, txtPass.Text, txtServer.Text);
                    connector.Initialize();
                    //must always be called, possibility to add pass,iv,salt by default this is already set
                    if (connector.isInitialized) {
                        if (connector.Connect()) {
                            //MessageBox.Show("Connected!!! : Creating Session using XPages URL");
                          

                            s = connector.GetSession(txtWebServiceURL.Text + "/xpJPIService.xsp/JPIService");
                            // Xpages nsf + /xpJPIService.xsp/JPIService
                            if (s != null && s.IsInitialized) {
                                MessageBox.Show("Session Retrieved!" + Environment.NewLine + GetArrayListAsString(Connector.ReturnMessages, Environment.NewLine));
                                if (string.IsNullOrEmpty(txtDbFilePath.Text)) {
                                    dbObj = s.GetDatabaseByID(txtDbRepId.Text, txtDbServer.Text);
                                } else {
                                    dbObj = s.GetDatabase(txtDbFilePath.Text, txtDbServer.Text);
                                }
                                if (dbObj != null && dbObj.IsInitialized) {
                                    MessageBox.Show("DatabaseObject Retrieved!" + Environment.NewLine + GetArrayListAsString(Connector.ReturnMessages, Environment.NewLine));
                                    MessageBox.Show("Template: " + dbObj.Template + Environment.NewLine + "Title: " + dbObj.Title + Environment.NewLine + "Size: " + dbObj.Size + Environment.NewLine + "URL: " + dbObj.Url);
                                  
                                    if (!string.IsNullOrEmpty(txtUnid.Text)) {
                                        if (txtUnid.Text.Contains(";")) {
                                            if (!dbObj.GetAllDocumentsAndFilesByUnids(txtUnid.Text)) {
                                                MessageBox.Show("Error while retrieving all documents by Unid " + GetArrayListAsString(Connector.ReturnMessages, Environment.NewLine));
                                            }
                                        } else {
                                            docObj = dbObj.GetDocumentAndFiles(txtUnid.Text);
                                            //dbObj.Documents.Add(docObj.UniversalID, docObj); // already added
                                        }

                                    } else {

                                        if (!string.IsNullOrEmpty(txtSearchField.Text) && !string.IsNullOrEmpty(txtSearchValue.Text)) {
                                            if (!dbObj.GetAllDocumentsAndFilesByKey(txtSearchField.Text, txtSearchValue.Text)) {
                                                MessageBox.Show("Error while retrieving all documents by search field & value " + GetArrayListAsString(Connector.ReturnMessages, Environment.NewLine));
                                            }
                                        } else {
                                            //search by formula
                                            if (!string.IsNullOrEmpty(txtSearchFormula.Text)) {
                                                if (!txtSearchFormula.Text.Contains(";")) {
                                                    docObj = dbObj.GetDocumentAndFilesByFormula(txtSearchFormula.Text);
                                                   // dbObj.Documents.Add(docObj.UniversalID, docObj);
                                                } else {
                                                    if (!dbObj.GetAllDocumentsAndFilesByFormula(txtSearchFormula.Text)) {
                                                        MessageBox.Show("Error while retrieving all documents by formula " + GetArrayListAsString(Connector.ReturnMessages, Environment.NewLine));
                                                    }
                                                }
                                                   
                                            } else {
                                                //search for all docs
                                                if (!dbObj.GetAllDocumentsAndFiles()) {
                                                    MessageBox.Show("Error while retrieving all documents from the database " + GetArrayListAsString(Connector.ReturnMessages, Environment.NewLine));
                                                }
                                            }
                                         
                                         
                                        }
                                    }

                                    if (dbObj.Documents != null && dbObj.Documents.Count > 0) {

                                        //string []  Unids = dbObj.Documents.Keys.ToArray();
                                        string unidlist = String.Join(Environment.NewLine, dbObj.Documents.Keys.ToArray());

                                        // dbObj.LoadDocumentFields()

                                        //MessageBox.Show(unidlist);

                                        StringBuilder sb = new StringBuilder();
                                        if(txtFields.Text != "")
                                        {
                                            // get all fields
                                            sb.AppendLine("Universal ID;" + txtFields.Text);
                                        }
                                     
                                        if (dbObj.LoadDocumentFields(txtFields.Text)) {
                                            //here we need to get a list of fields
                                            if (txtFields.Text == "")
                                            {
                                                // get all fields for first doc
                                                if(dbObj.Documents!=null && dbObj.Documents.Count > 0)
                                                {
                                                    sb.AppendLine("Universal ID;");
                                                    DocumentObject docObject = dbObj.Documents.ElementAt(0).Value;
                                                    foreach (KeyValuePair<string, FieldObject> kvp in docObject.Fields)
                                                    {
                                                        sb.Append(kvp.Key + ";");
                                                    }
                                                }
                                               
                                            }

                                            // string str = "";
                                            foreach (KeyValuePair<string, DocumentObject> kvpDoc in dbObj.Documents) {
                                                // str = str + "Unid: " + kvp.Value.UniversalID + "NoteID: " + kvp.Value.NoteID + Environment.NewLine + "Form: " + kvp.Value.Form + Environment.NewLine + "Size: " + kvp.Value.Size + Environment.NewLine + "URL: " + kvp.Value.Url + Environment.NewLine + Environment.NewLine;
                                                docObj = kvpDoc.Value;
                                                sb.Append(docObj.UniversalID + ";");

                                                if (docObj.Fields != null && docObj.Fields.Count > 0) {
                                                   
                                                    foreach (KeyValuePair<string, FieldObject> kvp in docObj.Fields) {
                                                            sb.Append(kvp.Value.GetValueAsString() + ";");
                                                        }
                                                    //sb.Append(Environment.NewLine);
                                                }
                                                if(docObj.Files != null && docObj.Files.Count > 0)
                                                {
                                                    foreach (KeyValuePair<string, FileObject> kvp in docObj.Files)
                                                    {
                                                        sb.Append(kvp.Value.Name  + " : " +  kvp.Value.URL + ";");
                                                      
                                                    }
                                                    
                                                }
                                                sb.Append(Environment.NewLine);
                                                //extract the files
                                                docObj.ExportFiles("C:\\JPI_Exported", true);
                                            } // end for
                                           
                                            if (System.IO.File.Exists(txtExportFile.Text + "_EXPORTED.csv")){
                                                System.IO.File.Delete(txtExportFile.Text + "_EXPORTED.csv");
                                            }
                                            System.IO.File.WriteAllText(txtExportFile.Text + "_EXPORTED.csv", sb.ToString());
                                            // if (docObj.GetFields(txtFields.Text)) { //jeNumber; jeProjectNumber; jeDisciplineCode;  wfRevisionCode;  jeTypeCode; jeTitle1
                                            MessageBox.Show("Retrieved the fields from the documents!" + Environment.NewLine + "Exported to : " + txtExportFile.Text + "_EXPORTED.csv"+ Environment.NewLine + GetArrayListAsString(Connector.ReturnMessages, Environment.NewLine));
                                        } else { // load doc fields
                                            MessageBox.Show("Unable to retrieve the Fields From the documents!" + Environment.NewLine + GetArrayListAsString(Connector.ReturnMessages, Environment.NewLine));
                                        }
                                    } else {
                                        MessageBox.Show("Unable to retrieve the DocumentObjects!" + Environment.NewLine + GetArrayListAsString(Connector.ReturnMessages, Environment.NewLine));
                                    }

                                } else {
                                    MessageBox.Show("Unable to retrieve the DatabaseObject!" + Environment.NewLine + GetArrayListAsString(Connector.ReturnMessages, Environment.NewLine));
                                }

                            } else {
                                MessageBox.Show("Session NOT Retrieved!" + Environment.NewLine + GetArrayListAsString(Connector.ReturnMessages, Environment.NewLine));
                            }
                        } else {
                            MessageBox.Show("Unable to Connect!!! " + Environment.NewLine + GetArrayListAsString(Connector.ReturnMessages, Environment.NewLine));
                        }
                    } // init
                } // fields not provided

            } catch (Exception ex) {
                MessageBox.Show(Common.GetErrorInfo(ex));
            }
        }
    }
}
