using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// An object representing a Notes Item (domino document field)
/// </summary>
public class FieldObject {

    #region Properties

    /// <summary>
    /// Name of the field
    /// </summary>
    public string Name { get; protected internal set; } = "";

    /// <summary>
    /// Value of the field
    /// </summary>
    public object Value { get; protected internal set; } = null;

    /// <summary>
    /// Type of the field
    /// </summary>
    public string Type { get; protected internal set; } = "";

    #endregion Properties

    #region Constructor

    /// <summary>
    /// Field Constructor
    /// </summary>
    /// <param name="name"></param>
    internal FieldObject(string name) {
        Name = name;
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Method to get the value of the field, by converting it to the provided type
    /// </summary>
    /// <returns></returns>
    public object GetValue() {
        try {
            // return converted value
            if (Type == "String") {
                return (String)Value;
            } else if (Type == "Number") {
                return (double)Value;
            } else if (Type == "Date") {
                return (DateTime)Value;
            } else if (Type == "List") {
                List<string> list = null;
                list = Value.ToString().Split(';').ToList<string>();
                return list;
            } else {
                return Value;
            }
        } catch (Exception ex) {
            Connector.ReturnMessages.Add("Unable to get the value of the field : " + Name);
            Connector.ReturnMessages.Add(Common.GetErrorInfo(ex));
            Connector.HasError = true;
            return null;
        }
    }

    /// <summary>
    /// Get the value of the field as string
    /// </summary>
    /// <returns></returns>
    public string GetValueAsString() {
        try {
            return (string)Value;
        } catch (Exception ex) {
            Connector.ReturnMessages.Add("Unable to get field : " + Name + " as string");
            Connector.ReturnMessages.Add(Common.GetErrorInfo(ex));
            Connector.HasError = true;
            return "";
        }
    }

    /// <summary>
    /// Get the value of the field as double
    /// </summary>
    /// <returns></returns>
    public double GetValueAsDouble() {
        try {
            return (double)Value;
        } catch (Exception ex) {
            Connector.ReturnMessages.Add("Unable to get field : " + Name + " as double");
            Connector.ReturnMessages.Add(Common.GetErrorInfo(ex));
            Connector.HasError = true;
            return 0;
        }
    }

    /// <summary>
    /// Get the value of the field as Date
    /// </summary>
    /// <returns></returns>
    public DateTime GetValueAsDate() {
        try {
            return (DateTime)Value;
        } catch (Exception ex) {
            Connector.ReturnMessages.Add("Unable to get field : " + Name + " as date");
            Connector.ReturnMessages.Add(Common.GetErrorInfo(ex));
            Connector.HasError = true;
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
            list = Value.ToString().Split(';').ToList<string>();
            return list;
        } catch (Exception ex) {
            Connector.ReturnMessages.Add("Unable to get field : " + Name + " as list");
            Connector.ReturnMessages.Add(Common.GetErrorInfo(ex));
            Connector.HasError = true;
            return null;
        }
    }

    #endregion Methods
}