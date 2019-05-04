﻿namespace SimpleInfra.Data
{
    using System;
    using System.Collections.Generic;

    public interface ISimpleRepoLogger
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Log Errors. </summary>
        ///
        /// <param name="e">        An Exception to process. </param>
        /// <param name="messages"> A variable-length parameters list containing messages. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void Error(Exception e, params string[] messages);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Logs any error messages. </summary>
        ///
        /// <param name="messages"> A variable-length parameters list containing messages. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void Error(params string[] messages);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Logs Debug the given dictionary. </summary>
        ///
        /// <param name="messages"> A variable-length parameters list containing messages. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void Debug(params string[] messages);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Logs Debug the given dictionary. </summary>
        ///
        /// <param name="dictionary">   The dictionary. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void Debug(Dictionary<string, string> dictionary);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Logs Info the given dictionary. </summary>
        ///
        /// <param name="messages"> A variable-length parameters list containing messages. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void Info(params string[] messages);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Logs Info the given dictionary. </summary>
        ///
        /// <param name="dictionary">   The dictionary. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void Info(Dictionary<string, string> dictionary);
    }
}