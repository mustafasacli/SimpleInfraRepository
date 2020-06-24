namespace SimpleInfra.Data
{
    using System;
    using System.Data.Entity;

    /// <summary>
    /// 
    /// </summary>
    public class DbContextDisposingEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public DbContextDisposingEventArgs(DbContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Gets DbContext object.
        /// </summary>
        public DbContext Context { get; private set; }
    }
}
