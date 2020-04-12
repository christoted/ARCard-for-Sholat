using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterObject 
{
    private string device_id,serial_number;

   public RegisterObject(string device_id, string serial_number)
    {
        this.device_id = device_id;
        this.serial_number = serial_number;
    }

     public string GetDevice_id()
    {
        return device_id;
    }  

    public string GetSerial_number()
    {
        return serial_number;
    }

}
