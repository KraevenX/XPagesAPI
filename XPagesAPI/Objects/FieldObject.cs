using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// An object representing a Domino item
/// </summary>
public class FieldObject {

    #region Variables

    private string _Name = "";
    private object _Value = null;
    private string _Type = "";

    #endregion

    #region Properties

    /// <summary>
    /// Name of the field
    /// </summary>
    public string Name {
        get {
            return _Name;
        }

        protected internal set {
            _Name = value;
        }
    }

    /// <summary>
    /// Value of the field
    /// </summary>
    public object Value {
        get {
            return _Value;
        }

        protected internal set {
            _Value = value;
        }
    }

    /// <summary>
    /// Type of the field
    /// </summary>
    public string Type {
        get {
            return _Type;
        }

        protected internal set {
            _Type = value;
        }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Field Constructor 
    /// </summary>
    /// <param name="name"></param>
    public FieldObject(string name) {
        _Name = name;
    }

    #endregion

    #region Methods
    
    /// <summary>
    /// Method to get the value of the field, by converting it to the provided type
    /// </summary>
    /// <returns></returns>
    public object getValue() {

        try {
            // return converted value
            if (_Type == "String") {
                return (String) _Value;
            } else if (_Type == "Number") {
                return (double) _Value;
            } else if (_Type == "Date") {
                return (DateTime) _Value;
            } else if (_Type == "List") {
                List<string> list = null;
                list = _Value.ToString().Split(';').ToList<string>();
                return list;
            } else {
                return _Value;
            }
        } catch (Exception ex) {

            Connector.ReturnMessages.Add("Unable to get the value of the field : " + _Name);
            Connector.ReturnMessages.Add(Common.GetErrorInfo(ex));
            Connector.hasError = true;
            return null;
        }
      
    }
  
   

    /// <summary>
    /// Get the value of the field as string
    /// </summary>
    /// <returns></returns>
    public string GetValueAsString() {
        try {

            return (string) _Value;
        } catch(Exception ex) {
            Connector.ReturnMessages.Add("Unable to get field : " + _Name + " as string");
            Connector.ReturnMessages.Add(Common.GetErrorInfo(ex));
            Connector.hasError = true;
            return "";
        }
        
    }

    /// <summary>
    /// Get the value of the field as double
    /// </summary>
    /// <returns></returns>
    public double GetValueAsDouble() {
        try {

            return (double) _Value;
        } catch(Exception ex) {
            Connector.ReturnMessages.Add("Unable to get field : " + _Name + " as double");
            Connector.ReturnMessages.Add(Common.GetErrorInfo(ex));
            Connector.hasError = true;
            return 0;
        }
       
    }

    /// <summary>
    /// Get the value of the field as Date
    /// </summary>
    /// <returns></returns>
    public DateTime GetValueAsDate() {
        try {
            return (DateTime) _Value;
        } catch (Exception ex) {
            Connector.ReturnMessages.Add("Unable to get field : " + _Name + " as date");
            Connector.ReturnMessages.Add(Common.GetErrorInfo(ex));
            Connector.hasError = true;
            return new DateTime(0);
        }
       
    }

    /// <summary>
    /// Get the value of the field as list of strings
    /// </summary>
    /// <returns></returns>
    public List<string> GetValueAsList() {
        try {
            List<string> list = null;
            list = _Value.ToString().Split(';').ToList<string>();
            return list;
        } catch (Exception ex) {
            Connector.ReturnMessages.Add("Unable to get field : " + _Name + " as list");
            Connector.ReturnMessages.Add(Common.GetErrorInfo(ex));
            Connector.hasError = true;
            return null;
        }
     }

    #endregion

}