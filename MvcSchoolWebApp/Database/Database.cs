
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using FinalTemplate.source.Validation;
using MvcSchoolWebApp.Controllers;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Database
{
    public class Database : Controller
    {


        #region Constructors
        //To create new object with your connection string, just pass connection string as parameter when creating object.
        public Database(string ConnectionStringName)
        {
            //Calling private method MakeConnectionString
            MakeConnectionString(ConnectionStringName);
        }

        public Database()
        {
        }
        #endregion
        #region SQL Data Providers
        public SqlConnection obj_sqlconnection;
        public SqlCommand obj_sqlcommand;
        public SqlDataReader obj_reader;
        public SqlParameter[] obj_sqlparameter;
        public SqlDataAdapter obj_adapter;
        public DataSet obj_dataset;
        #endregion
        #region Private Data Members
        //This variable contains the actual connection string to the database. It is advices to change it only once.
        private string _connectionString = string.Empty;
        //String builder class object QUERY is used to carry the entire query.
        //This object will hold query for insert update select and delete
        StringBuilder Query = new StringBuilder();
        #endregion
        #region Properties
        //This property will set the connection string for database
        public string ConnectionString
        {
            get
            {   //if _connectionString is already created or set, only then it will return the value of _connectionString
                if (_connectionString != string.Empty && _connectionString != "" && _connectionString != null)
                    return _connectionString;
                else
                    return string.Empty;
            }
            //When you want to set the connection string set block is called.
            set
            {   //this line sets the connection string to the _connectionString data member for the first time.
                if (_connectionString == string.Empty || _connectionString == "" || _connectionString == null)
                    _connectionString = value;
            }
        }
        //This will return the current sql connection object.
        public SqlConnection GetCurrentConnection
        {
            get { return obj_sqlconnection; }
            set { obj_sqlconnection = value; }
        }

        #endregion
        //This region contains all the methods which are require to perform any DML operation.
        #region Private DML Methods
        //Used to assign ConnectionString value to ConnectionString Property.
        //Which will carry current connection string value through out the object of database class.
        private void MakeConnectionString(string p_ConnectionString)
        {
            ConnectionString = p_ConnectionString;
        }
        //This method initialize the SQL command object with connection and command text(query)
        public void InitializeSQLCommandObject(SqlConnection SQLConnectionObject, string CommandText)
        {
            obj_sqlcommand = new SqlCommand();
            obj_sqlcommand.Connection = GetCurrentConnection;
            obj_sqlcommand.CommandType = CommandType.Text;
            obj_sqlcommand.CommandText = CommandText;
        }
        public void InitializeSQLCommandObject(SqlConnection sqlConectioConnection, string CommandText, bool isSP)
        {
            obj_sqlcommand = new SqlCommand();
            obj_sqlcommand.Connection = GetCurrentConnection;
            obj_sqlcommand.CommandType = CommandType.StoredProcedure;
            obj_sqlcommand.CommandText = CommandText;
        }
        #endregion
        #region Public DML Methods
        #region Database Connection Methods
        //This method will initialize the obj_connection object with parameterized connection string.
        public void CreateConnection()
        {
            var ConfiguredString = ConfigurationManager.ConnectionStrings[ConnectionString].ConnectionString;
            obj_sqlconnection = new SqlConnection(ConfiguredString);

        }


        //Open database connection.    
        [HandleError]
        public void OpenConnection()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            //if (con.State == ConnectionState.Closed)
            //{
                try
                {
                    obj_sqlconnection.Open();
                }
                catch (Exception ex)
                {
                    throw new Exception("Bad or No Connection Available");
                }
            //}


        }
        //Close database connection.
        public void CloseConnection()
        {
            obj_sqlconnection.Close();
            obj_sqlconnection.Dispose();
        }
        #endregion
        //this method will execute insert query.
        //This method is fully dynamic and support any type of query and execute it safely.
        public int InsertQuery(string TableName, string[] ColumnNames, object[] ColumnValues)
        {
            string CurrentColumnType, CurrentColumn = string.Empty;
            int RowsAffected = 0;
            #region Generating dynamic query
            //Generating dynamic query depending on number of columns and object array values.                
            int i = 0;


            Query.Append("Insert into ");
            //Adding name of the table, passed by parameter value.
            Query.Append(TableName);
            Query.Append("(");
            //Appending column names in query object.
            for (i = 0; i < ColumnNames.Length; i++)
            {
                //Appending , after each column name till ColumnLength-1
                if (i + 1 == ColumnNames.Length)
                    Query.Append(ColumnNames[i]);
                else
                    Query.Append(ColumnNames[i] + ",");
            }
            //Following query syntax
            Query.Append(") values(");
            //Adding parameters for the values 
            for (i = 0; i < ColumnValues.Length; i++)
            {
                //Same as appending line # 68
                if (i + 1 == ColumnValues.Length)
                    Query.Append("@" + ColumnNames[i]);
                else
                    Query.Append("@" + ColumnNames[i] + ",");
            }
            //Query syntax completes
            Query.Append(");");
            #endregion
            #region Query Execution
            //Creating connection to database
            CreateConnection();
            #region Execute Query
            try
            {
                InitializeSQLCommandObject(GetCurrentConnection, Query.ToString());

                //check the current column data type and convert object array parameter's value to that data type.
                for (i = 0; i < ColumnNames.Length; i++)
                {
                    //Getting current data type
                    CurrentColumnType = ColumnValues[i].GetType().ToString();
                    //converting parameter value to array to pass to the FilerBlackListKeywords method.
                    string[] columnArray = Convert.ToString(ColumnValues[i]).Split(' ');
                    //string[] columnArray = { "" + ColumnValues[i] + "" };

                    //Checking current data type.                        
                    if (CurrentColumnType == "System.Text.StringBuilder" || CurrentColumnType == "System.String")
                    {   //if black listed keywords are filtered successfully. Then converting actual value to respective column type match as sql server database table
                        if (Jvalidate.FilterBlackLIstKeywords(columnArray))
                            this.obj_sqlcommand.Parameters.AddWithValue("@" + ColumnNames[i].ToString(), Jvalidate.RemoveHtmlTags(ColumnValues[i].ToString()));
                        else
                            break;
                    }
                    else if (CurrentColumnType == "Syste.Decimal")
                    {
                        //if black listed keywords are filtered successfully. Then converting actual value to respective column type match as sql server database table
                        if (Jvalidate.FilterBlackLIstKeywords(columnArray))
                            this.obj_sqlcommand.Parameters.AddWithValue("@" + ColumnNames[i].ToString(), Convert.ToDecimal(Jvalidate.RemoveHtmlTags(ColumnValues[i].ToString())));
                        else
                            break;
                    }

                    else if (CurrentColumnType == "System.Boolean")
                    {
                        //if black listed keywords are filtered successfully. Then converting actual value to respective column type match as sql server database table
                        if (Jvalidate.FilterBlackLIstKeywords(columnArray))
                        {
                            this.obj_sqlcommand.Parameters.AddWithValue("@" + ColumnNames[i].ToString(),
                                Convert.ToBoolean(Jvalidate.RemoveHtmlTags(ColumnValues[i].ToString())));
                        }
                        else
                            break;
                    }
                    else if (CurrentColumnType == "System.Int32")
                    {
                        //if black listed keywords are filtered successfully. Then converting actual value to respective column type match as sql server database table
                        if (Jvalidate.FilterBlackLIstKeywords(columnArray))
                        {
                            this.obj_sqlcommand.Parameters.AddWithValue("@" + ColumnNames[i].ToString(),
                                Convert.ToInt32(Jvalidate.RemoveHtmlTags(ColumnValues[i].ToString())));



                        }
                        else
                            break;
                    }
                }
                //Opening connection   
                OpenConnection();
                //Executing query.
                RowsAffected = this.obj_sqlcommand.ExecuteNonQuery();
            }
            //Finally closing the connetion to database
            finally
            {
                CloseConnection();
            }
            #endregion
            #endregion
            //Return number of records affected by query execution.
            return RowsAffected;
        }
        public int UpdateQuery(string TableName, string[] ColumnNames, object[] ColumnValues, string WhereClause, object WhereClauseValue)
        {
            int RowsAffacted = 0;
            #region Creating update query
            Query.Clear();
            Query.Append("update ");
            Query.Append(TableName);
            Query.Append(" set ");
            for (int i = 0; i < ColumnNames.Length; i++)
            {
                if (i + 1 == ColumnNames.Length)
                {
                    Query.Append(ColumnNames[i]);
                    Query.Append("=");
                    Query.Append("@" + ColumnNames[i]);
                }
                else
                {
                    Query.Append(ColumnNames[i]);
                    Query.Append("=");
                    Query.Append("@" + ColumnNames[i]);
                    Query.Append(",");

                }
            }
            Query.Append(" where ");
            Query.Append(WhereClause);
            Query.Append("=");
            Query.Append("@" + WhereClause);
            #endregion
            #region Execution query
            CreateConnection();
            try
            {
                InitializeSQLCommandObject(GetCurrentConnection, Query.ToString());
                for (int i = 0; i < ColumnNames.Length; i++)
                {
                    string CurrentColumn = ColumnValues[i].GetType().ToString();
                    string[] columnArray = Convert.ToString(ColumnValues[i]).Split(' ');
                    if (CurrentColumn == "System.String" || CurrentColumn == "System.Text.StringBuilder")
                    {
                        if (Jvalidate.FilterBlackLIstKeywords(columnArray))
                            this.obj_sqlcommand.Parameters.AddWithValue("@" + ColumnNames[i], Jvalidate.RemoveHtmlTags(ColumnValues[i].ToString()));
                        else
                            break;
                    }
                    else if (CurrentColumn == "System.Int32")
                    {
                        if (Jvalidate.FilterBlackLIstKeywords(columnArray))
                            this.obj_sqlcommand.Parameters.AddWithValue("@" + ColumnNames[i], Jvalidate.RemoveHtmlTags((Convert.ToInt32(ColumnValues[i])).ToString()));
                        else
                            break;
                    }
                    else if (CurrentColumn == "System.Decimal")
                    {
                        if (Jvalidate.FilterBlackLIstKeywords(columnArray))
                            this.obj_sqlcommand.Parameters.AddWithValue("@" + ColumnNames[i], Jvalidate.RemoveHtmlTags((Convert.ToDecimal(ColumnValues[i])).ToString()));
                        else
                            break;
                    }
                }

                string ClauseValueType = WhereClauseValue.GetType().ToString();
                string[] ClauseArray = { "" + WhereClauseValue + "" };
                if (ClauseValueType == "System.String" || ClauseValueType == "System.Text.StringBuilder")
                {
                    if (Jvalidate.FilterBlackLIstKeywords(ClauseArray))
                        this.obj_sqlcommand.Parameters.AddWithValue("@" + WhereClause, Jvalidate.RemoveHtmlTags(WhereClauseValue.ToString()));
                }
                else if (ClauseValueType == "System.Int32")
                {
                    if (Jvalidate.FilterBlackLIstKeywords(ClauseArray))
                        this.obj_sqlcommand.Parameters.AddWithValue("@" + WhereClause, Convert.ToInt32(Jvalidate.RemoveHtmlTags(WhereClauseValue.ToString())));
                }
                else if (ClauseValueType == "System.Decimal")
                {
                    if (Jvalidate.FilterBlackLIstKeywords(ClauseArray))
                        this.obj_sqlcommand.Parameters.AddWithValue("@" + WhereClause, Jvalidate.RemoveHtmlTags((Convert.ToDecimal(WhereClauseValue)).ToString()));
                }
                OpenConnection();
                RowsAffacted = this.obj_sqlcommand.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                CloseConnection();
            }
            #endregion

            return RowsAffacted;
        }
        public int DeleteQuery(string TableName, string WhereClause, object WhereClauseValue)
        {
            int RowsAffected = 0;
            #region Creating Query
            Query.Append("delete ");
            Query.Append(TableName);
            Query.Append(" where ");
            Query.Append(WhereClause);
            Query.Append("=");
            Query.Append("@" + WhereClause);
            #endregion
            #region Executing Query
            CreateConnection();
            try
            {
                InitializeSQLCommandObject(GetCurrentConnection, Query.ToString());
                string CurrentColumn = WhereClauseValue.GetType().ToString();
                string[] columnArray = { "" + WhereClauseValue + "" };
                if (CurrentColumn == "System.String" || CurrentColumn == "System.Text.StringBuilder")
                {
                    if (Jvalidate.FilterBlackLIstKeywords(columnArray))
                        this.obj_sqlcommand.Parameters.AddWithValue("@" + WhereClause, Jvalidate.RemoveHtmlTags(WhereClauseValue.ToString()));
                }
                else if (CurrentColumn == "System.Int32")
                {
                    if (Jvalidate.FilterBlackLIstKeywords(columnArray))
                        this.obj_sqlcommand.Parameters.AddWithValue("@" + WhereClause, Convert.ToInt32(Jvalidate.RemoveHtmlTags(WhereClauseValue.ToString())));
                }
                else if (CurrentColumn == "System.Decimal")
                {
                    if (Jvalidate.FilterBlackLIstKeywords(columnArray))
                        this.obj_sqlcommand.Parameters.AddWithValue("@" + WhereClause, Jvalidate.RemoveHtmlTags((Convert.ToDecimal(WhereClauseValue)).ToString()));
                }
                OpenConnection();
                RowsAffected = this.obj_sqlcommand.ExecuteNonQuery();

            }
            catch { }
            finally { CloseConnection(); }
            #endregion
            return RowsAffected;
        }
        public string[,] SelectQuery(string TableName, string[] Columns)
        {

            int columnCount = 0;
            string[,] finalResult = { { }, { } };
            string CountQuery = "select count(*) from " + TableName;
            Query.Clear();
            Query.Append("Select ");
            for (int i = 0; i < Columns.Length; i++)
            {
                if (i + 1 == Columns.Length)
                    Query.Append(Columns[i]);
                else
                    Query.Append(Columns[i] + ",");
            }
            Query.Append(" from ");
            Query.Append(TableName);

            CreateConnection();
            try
            {
                OpenConnection();
                InitializeSQLCommandObject(GetCurrentConnection, Query.ToString());
                this.obj_sqlcommand.CommandText = CountQuery;
                var rowCount = this.obj_sqlcommand.ExecuteScalar();
                columnCount = Columns.Length;
                int lastValue = 0;
                finalResult = new string[Convert.ToInt32(rowCount), columnCount];
                this.obj_sqlcommand.CommandText = Query.ToString();
                this.obj_reader = this.obj_sqlcommand.ExecuteReader();
                if (this.obj_reader.HasRows)
                {
                    while (obj_reader.Read())
                    {

                        for (int i = 0; i < columnCount; i++)
                        {
                            finalResult[lastValue, i] = obj_reader[i].ToString();
                        }
                        lastValue++;
                    }
                }
                else
                    finalResult[0, 0] = "null";
            }
            finally
            {
                CloseConnection();
                obj_reader.Dispose();
                obj_sqlcommand.Dispose();
            }

            return finalResult;

        }
        public string[,] SelectQuery(string TableName, string[] Columns, string[] whereColumn, string[] whereOperator, string[] whereValue, string[] multipleWhereClauseOperator)
        {

            int columnCount = 0;
            string[,] finalResult = { { }, { } };
            string CountQuery = "select count(*) from " + TableName;
            Query.Clear();
            Query.Append("Select ");
            for (int i = 0; i < Columns.Length; i++)
            {
                if (i + 1 == Columns.Length)
                    Query.Append(Columns[i]);
                else
                    Query.Append(Columns[i] + ",");
            }
            Query.Append(" from ");
            Query.Append(TableName);
            Query.Append(" where ");
            for (int i = 0; i < whereColumn.Length; i++)
            {
                if (whereColumn.Length == 1)
                {
                    Query.Append(" " + whereColumn[i] + " " + whereOperator[i] + " " + whereValue[i]);
                }
                else
                {
                    if (i == (whereColumn.Length - 1))
                    {
                        Query.Append(" " + whereColumn[i] + " " + whereOperator[i] + " " + whereValue[i]);
                    }
                    else
                    {
                        Query.Append(" " + whereColumn[i] + " " + whereOperator[i] + " " + whereValue[i] + multipleWhereClauseOperator[i] + " ");
                    }
                }
            }

            CreateConnection();
            try
            {
                OpenConnection();
                InitializeSQLCommandObject(GetCurrentConnection, Query.ToString());
                this.obj_sqlcommand.CommandText = CountQuery;
                var rowCount = this.obj_sqlcommand.ExecuteScalar();
                columnCount = Columns.Length;
                int lastValue = 0;
                finalResult = new string[Convert.ToInt32(rowCount), columnCount];
                this.obj_sqlcommand.CommandText = Query.ToString();
                this.obj_reader = this.obj_sqlcommand.ExecuteReader();
                if (obj_reader.HasRows)
                {
                    while (obj_reader.Read())
                    {

                        for (int i = 0; i < columnCount; i++)
                        {
                            finalResult[lastValue, i] = obj_reader[i].ToString();
                        }
                        lastValue++;
                    }
                }


            }
            catch (Exception exception)
            {
                Response.Write(exception.ToString());
            }
            finally
            {
                CloseConnection();
                //obj_reader.Dispose();
                obj_sqlcommand.Dispose();
            }

            return finalResult;

        }


        #region StoreProcedures

        public bool ExecuteProcedure(string spName, string[] parametersName, object[] parametersValues)
        {
            if (parametersName.Length == parametersValues.Length)
            {
                int outputParameterCount = 0;
                int ParameterLength = 0;
                string currentParameterValue = string.Empty;
                CreateConnection();
                try
                {
                    InitializeSQLCommandObject(GetCurrentConnection, spName, true);
                    //calculating output parameters
                    for (int i = 0; i < parametersValues.Length; i++)
                    {
                        if (parametersValues[i] == "output" || parametersValues[i] == "Output" || parametersValues[i] == "OUTPUT" || parametersValues[i] == "OutPut" || parametersValues[i] == "OUT" || parametersValues[i] == "Out" || parametersValues[i] == "out")
                        {
                            outputParameterCount++;
                        }
                    }
                    //initialize the array for output parameters.
                    obj_sqlparameter = new SqlParameter[outputParameterCount];
                    for (int i = 0; i < parametersName.Length; i++)
                    {
                        currentParameterValue = parametersValues.GetType().ToString();
                        if (parametersValues[i] == "output" || parametersValues[i] == "Output" || parametersValues[i] == "OUTPUT" || parametersValues[i] == "OutPut")
                        {
                            obj_sqlparameter[ParameterLength].ParameterName = "@" + parametersName[i].ToString();
                            obj_sqlparameter[ParameterLength].Direction = ParameterDirection.Output;
                            if (currentParameterValue == "System.Text")
                            {
                                obj_sqlparameter[ParameterLength].SqlDbType = SqlDbType.VarChar;
                                obj_sqlparameter[ParameterLength].Size = currentParameterValue.Max();
                            }
                            else if (currentParameterValue == "System.Int" || currentParameterValue == "System.Int32")
                                obj_sqlparameter[ParameterLength].SqlDbType = SqlDbType.Int;
                            else if (currentParameterValue == "System.Decimal")
                                obj_sqlparameter[ParameterLength].SqlDbType = SqlDbType.Decimal;
                            else if (currentParameterValue == "System.Date")
                                obj_sqlparameter[ParameterLength].SqlDbType = SqlDbType.Date;
                            //adding the parameter to sql command object
                            obj_sqlcommand.Parameters.Add(obj_sqlparameter[ParameterLength]);
                            ParameterLength++;
                        }
                        else
                        {
                            if (currentParameterValue == "System.Text")
                                obj_sqlcommand.Parameters.AddWithValue("@" + parametersName[i], parametersValues[i].ToString());
                            else if (currentParameterValue == "System.Int")
                                obj_sqlcommand.Parameters.AddWithValue("@" + parametersName[i], Convert.ToInt32(parametersValues[i]));
                            else if (currentParameterValue == "System.Decimal")
                                obj_sqlcommand.Parameters.AddWithValue("@" + parametersName[i], Convert.ToDecimal(parametersValues[i]));
                            else if (currentParameterValue == "System.Date")
                                obj_sqlcommand.Parameters.AddWithValue("@" + parametersName[i], Convert.ToDateTime(parametersValues[i]));
                        }
                        OpenConnection();
                        int rowsAffected = obj_sqlcommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            ParameterLength = 0;
                        }
                    }
                }
                catch (Exception exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        #endregion
        #endregion
        #region TableQueries
        public string GetAuthorizedID(string username, string password)
        {
            string returnvalue = string.Empty;
            string[] username_array = { username };
            string[] password_array = { password };
            if (Jvalidate.FilterBlackLIstKeywords(username_array) && Jvalidate.FilterBlackLIstKeywords(password_array))
            {
                Query.Clear();
                Query.Append("select authorized_id from tbl_authorized_users where username = '" + username + "' and password = '" + password + "'");
                try
                {
                    CreateConnection();
                    InitializeSQLCommandObject(obj_sqlconnection, Query.ToString());
                    OpenConnection();
                    obj_reader = obj_sqlcommand.ExecuteReader();
                    if (obj_reader.HasRows)
                    {
                        while (obj_reader.Read())
                        {
                            returnvalue = obj_reader[0].ToString();
                        }
                    }
                    else
                    {
                        returnvalue = "no rows found";
                    }

                }
                catch (Exception ex)
                {
                    returnvalue = ex.ToString();
                }
                finally
                {
                    CloseConnection();
                    obj_reader.Dispose();
                    obj_sqlcommand.Dispose();
                    Query.Clear();
                }
            }
            return returnvalue.ToString();

        }
        //insecure method
        public string GetLastValueByColumnName(string columnName, string tableName)
        {
            string query = "select top 1 " + columnName + " from " + tableName + " order by " + columnName + " desc;";
            CreateConnection();
            InitializeSQLCommandObject(GetCurrentConnection, query);
            try
            {
                string lastvalue = string.Empty;
                OpenConnection();
                obj_reader = obj_sqlcommand.ExecuteReader();
                if (obj_reader.HasRows)
                {
                    while (obj_reader.Read())
                    {
                        lastvalue = obj_reader[0].ToString();
                    }
                    return lastvalue;
                }
                else
                {
                    query = "0";
                    return query;
                }
            }
            catch (Exception exception)
            {
                query = exception.ToString();
            }
            finally
            {
                CloseConnection();
                obj_reader.Dispose();
                obj_reader.Close();
                obj_sqlcommand.Dispose();
            }
            return query;
        }

        #endregion
    }
}
