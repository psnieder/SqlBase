using System;
using System.Data.SqlClient;

namespace BaseClasses
{
    public abstract class SqlBase
    {
        #region Properties
        private SqlConnection Con { get; }
        private SqlTransaction Transaction { get; set; }
        private bool Rollback { get; set; }
        protected SqlCommand Command { get; }
        #endregion

        #region Constructors
        protected SqlBase()
        {
            Con = new SqlConnection();
            Rollback = false;
            Command = new SqlCommand();
            Command.Connection = Con;
        }

        protected SqlBase(string connectionString)
        {
            Con = new SqlConnection(connectionString);
            Rollback = false;
            Command = new SqlCommand();
            Command.Connection = Con;
        }
        #endregion

        #region Methods 
        public void ClearParameters()
        {
            Command.Parameters.Clear();
        }
        #endregion

        #region Transactions
        public void BeginTransaction()
        {
            Con.Open();
            Transaction = Con.BeginTransaction();
            Command.Transaction = Transaction;
        }

        public bool CommitTransaction()
        {
            if (Transaction is null)
            {
                throw new Exception("Cannot commit without an open transaction");
            }

            /* Commit transaction */
            try
            {
                Transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error committing transaction: " + ex.GetType() + ", Message: " + ex.Message);
                Rollback = true;
                return false;
            }

            Con.Close();
            Console.WriteLine("SQL transaction committed.");
            return true;
        }

        public void RollbackTransaction()
        {
            if (Transaction is null)
            {
                throw new Exception("Cannot rollback without an open transaction");
            }

            if (!Rollback)
            {
                throw new Exception("Cannot rollback a transaction that was not attempted to be committed");
            }

            /* Rollback transaction */
            try
            {
                Transaction.Rollback();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error rolling back transaction: " + ex.GetType() + ", Message: " + ex.Message);
            }

            Rollback = false;
            Con.Close();
            Console.WriteLine("SQL transaction rolled back.");
        }
        #endregion
    }
}
