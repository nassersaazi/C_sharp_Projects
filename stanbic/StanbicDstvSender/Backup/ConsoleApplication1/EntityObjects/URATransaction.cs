using System;
using System.Data;
using System.Configuration;
using ConsoleApplication1.EntityObjects;

/// <summary>
/// Summary description for URATransaction
/// </summary>
public class URATransaction : Transaction
{
    private string tin, branchCode, bankCode, status;

    public string TIN
    {
        get
        {
            return tin;
        }
        set
        {
            tin = value;
        }
    }

    public string Status
    {
        get
        {
            return status;
        }
        set
        {
            status = value;
        }
    }

    public string BranchCode
    {
        get
        {
            return branchCode;
        }
        set
        {
            branchCode = value;
        }
    }

    public string BankCode
    {
        get
        {
            return bankCode;
        }
        set
        {
            bankCode = value;
        }
    }
}
