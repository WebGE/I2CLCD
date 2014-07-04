// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// PB avec carte NetDuino lors de l'envoi de plusieurs transactions en écriture
// A partir de la deuxième, les deux premiers octet du buffer ne sont pas pris en compte !
// Solution : rajout de deux octets morts !
// Surveiller les prochaines versions du SDK 
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace BatronLCD
{
    class I2CLcd
    {
        // Attributs
        private I2CDevice.Configuration ConfigI2CLcd;
        private I2CDevice BusI2C;

        public enum CursorType
        {
            Hide,
            Underline,
            Blink
        };


        // Constructeurs
        // this constructor assumes the default factory Slave Address = 0x3B
        public I2CLcd()
        {
            ConfigI2CLcd = new I2CDevice.Configuration(0x3B, 100);
        }

        // This constructor allows user to specify the Slave Address and bus frequency = 100khz
        public I2CLcd(ushort I2C_Add_7bits)
        {
            ConfigI2CLcd = new I2CDevice.Configuration(I2C_Add_7bits, 100);
        }

        // This constructor allows user to specify the Slave Address and bus frequency
        public I2CLcd(ushort I2C_Add_7bits, UInt16 FreqBusI2C)
        {
            ConfigI2CLcd = new I2CDevice.Configuration(I2C_Add_7bits, 100);
        }

        // Méthodes publiques
        public void Init()
        {
            // Création d'un buffer et d'une transaction pour l'accès au circuit en écriture
            byte[] outbuffer = new byte[] { 0x00, 0x34, 0x0d, 0x06, 0x35, 0x04, 0x10, 0x42, 0x9F, 0x34, 0x83, 0x02 };
            I2CDevice.I2CTransaction WriteTransaction = I2CDevice.CreateWriteTransaction(outbuffer);
            // Tableaux des transactions 
            I2CDevice.I2CTransaction[] T_WriteBytes = new I2CDevice.I2CTransaction[] { WriteTransaction };
            // Exécution de la transaction
            BusI2C = new I2CDevice(ConfigI2CLcd); // Connexion virtuelle de l'objet Lcd  au bus I2C 
            BusI2C.Execute(T_WriteBytes, 1000); 
            BusI2C.Dispose(); // Déconnexion virtuelle de l'objet Lcd du bus I2C
        }

        // Three cursor modes
        public void SelectCursor(CursorType posCursor)
        {
            // Création d'un buffer et d'une transaction pour l'accès au circuit en écriture
            byte[] outbuffer = new byte[] { 0, (byte)(4 + posCursor) };
            I2CDevice.I2CTransaction WriteTransaction = I2CDevice.CreateWriteTransaction(outbuffer);
            // Tableaux des transactions 
            I2CDevice.I2CTransaction[] T_WriteBytes = new I2CDevice.I2CTransaction[] { WriteTransaction };
            // Exécution de la transaction
            BusI2C = new I2CDevice(ConfigI2CLcd); // Connexion virtuelle de l'objet Lcd  au bus I2C 
            BusI2C.Execute(T_WriteBytes, 1000);
            BusI2C.Dispose(); // Déconnexion virtuelle de l'objet Lcd du bus I2C                 
        }

        // Set brightness  31 dark -> 0 light
        public void SetBacklight(byte bright)
        {
            byte BLight = 0x40;
            BLight += (byte)(bright & 0x1F);
            // Création d'un buffer et d'une transaction pour l'accès au circuit en écriture
            byte[] outbuffer = new byte[] { 0, 0x35, 0x42, BLight, 0x34, 0xc };
            I2CDevice.I2CTransaction WriteTransaction = I2CDevice.CreateWriteTransaction(outbuffer);
            // Tableaux des transactions 
            I2CDevice.I2CTransaction[] T_WriteBytes = new I2CDevice.I2CTransaction[] { WriteTransaction };
            // Exécution de la transaction
            BusI2C = new I2CDevice(ConfigI2CLcd); // Connexion virtuelle de l'objet Lcd  au bus I2C 
            BusI2C.Execute(T_WriteBytes, 1000);
            BusI2C.Dispose(); // Déconnexion virtuelle de l'objet Lcd du bus I2C   
        }

        // Goto start of line 
        public void LineBegin(byte x)
        {
            byte addr = 0x80;
            if (x == 1) addr += 0x40;
            // Création d'un buffer et d'une transaction pour l'accès au circuit en écriture
            byte[] outbuffer = new byte[] { 0, addr };
            I2CDevice.I2CTransaction WriteTransaction = I2CDevice.CreateWriteTransaction(outbuffer);
            // Tableaux des transactions 
            I2CDevice.I2CTransaction[] T_WriteBytes = new I2CDevice.I2CTransaction[] { WriteTransaction };
            // Exécution de la transaction
            BusI2C = new I2CDevice(ConfigI2CLcd); // Connexion virtuelle de l'objet Lcd  au bus I2C 
            BusI2C.Execute(T_WriteBytes, 1000);
            BusI2C.Dispose(); // Déconnexion virtuelle de l'objet Lcd du bus I2C   
        }

        // Write a line of text at x,y
        public void PutString(byte x_pos, byte y_pos, string Text)
        {
            
            byte addr = 0x80;
            if (x_pos < 17) addr += x_pos;                              // This is for 16 x 2, adjust as nessesary
            if (y_pos == 1) addr += 0x40;
            byte[] txt = System.Text.Encoding.UTF8.GetBytes((byte)'0' + (byte)'0' + (byte)'0' + Text);
            for (byte z = 3; z < (Text.Length + 3); z++)                // All characters have to be moved up!!
            { txt[z] += 128; }
           txt[2] = 0x40;  // R/S to Data

            // Création d'un buffer et de deux transactions pour l'accès au circuit en écriture
            byte[] outbuffer = new byte[] { 0, addr };
            I2CDevice.I2CTransaction WriteBytes = I2CDevice.CreateWriteTransaction(outbuffer); // set address
            I2CDevice.I2CTransaction WriteText = I2CDevice.CreateWriteTransaction(txt); // Print line
            // Tableaux des transactions 
            I2CDevice.I2CTransaction[] T_WriteBytes = new I2CDevice.I2CTransaction[] { WriteBytes, WriteText };
            // Exécution de la transaction
            BusI2C = new I2CDevice(ConfigI2CLcd); // Connexion virtuelle de l'objet Lcd  au bus I2C 
            BusI2C.Execute(T_WriteBytes, 1000);
            BusI2C.Dispose(); // Déconnexion virtuelle de l'objet Lcd du bus I2C   
        }

        // Single character write at x,y
        public void PutChar(byte x_pos, byte y_pos, byte z_char)
        {
            byte addr = 0x80;                 // This is for 16 x 2, adjust as nessesary
            if (x_pos < 17) addr += x_pos;    // x dir
            if (y_pos == 1) addr += 0x40;     // y dir
   
            // Création d'un buffer et de deux transactions pour l'accès au circuit en écriture
            byte[] outbuffer = new byte[] { 0, addr };
            I2CDevice.I2CTransaction WriteBytes = I2CDevice.CreateWriteTransaction(outbuffer); // set address (R/S to Cmd )
            I2CDevice.I2CTransaction WriteText = I2CDevice.CreateWriteTransaction(new byte[] { 0, 0, 0x40, z_char });  // single char write
            // Tableaux des transactions 
            I2CDevice.I2CTransaction[] T_WriteBytes = new I2CDevice.I2CTransaction[] { WriteBytes, WriteText };
            // Exécution de la transaction
            BusI2C = new I2CDevice(ConfigI2CLcd); // Connexion virtuelle de l'objet Lcd  au bus I2C 
            BusI2C.Execute(T_WriteBytes, 1000);
            BusI2C.Dispose(); // Déconnexion virtuelle de l'objet Lcd du bus I2C    
        }

        // Clear screen with 0xA0 not 0x20
        public void ClearScreen()                     // If you use built in Clr
        {                                             // the screen doen't clear
            byte[] clr = new byte[19];                // you just get arrows
            clr[0] = 0; clr[1] = 0; clr[2] = 0x40;    // R/S to Data
            for (byte x = 3; x < 19; x++)             // Space is not 0x20 on this unit!
            { clr[x] = 0xA0; }
            // Création d'un buffer et de quatre transactions pour l'accès au circuit en écriture
            I2CDevice.I2CTransaction SetTop = I2CDevice.CreateWriteTransaction(new byte[] { 0, 0x80 }); // set top line (R/S to Cmd )
            I2CDevice.I2CTransaction Setclr = I2CDevice.CreateWriteTransaction(clr); 
            I2CDevice.I2CTransaction SetBottom = I2CDevice.CreateWriteTransaction(new byte[] { 0, 0, 0, 0xC0 });  // set bottom line (R/S to Cmd )
            I2CDevice.I2CTransaction SetHome = I2CDevice.CreateWriteTransaction(new byte[] { 0, 0, 0, 0x80 }); // Cursor to home (R/S to Cmd )
            // Tableaux des transactions 
            I2CDevice.I2CTransaction[] T_WriteBytes = new I2CDevice.I2CTransaction[] { SetTop, Setclr, SetBottom, Setclr, SetHome };
            // Exécution de la transaction
            BusI2C = new I2CDevice(ConfigI2CLcd); // Connexion virtuelle de l'objet Lcd  au bus I2C 
            BusI2C.Execute(T_WriteBytes, 1000);
            BusI2C.Dispose(); // Déconnexion virtuelle de l'objet Lcd du bus I2C 
        }
    }
}