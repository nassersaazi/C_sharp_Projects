using System;
using System.Collections.Generic;
using System.Text;


public class BankResponse : Response
{
    public string BankId;
    public string BankBranch;

    public bool IsSuccessfullAtBank()
    {
        //success or duplicate
        if (this.StatusCode.Equals("0") || StatusCode.Equals("21"))
        {
            return true;
        }
        return false;
    }
}

