

<?php
//Generate the privatekey.pem
openssl genrsa -aes256 -passout pass:ugabet@12345 -out Ugabetp.pem 2048

//Obtain a .cer public key from the privatekey.pem
Openssl req -new -x509 -key Ugabetp.pem -out UgaBetCert.cer -days 1825
//Generate a corresponding .cer public key from the .cer
OPENSSL x509 -inform PEM -in UgabetCert.cer -out UgabetCertificate.crt


//Use the .cer and the privatekey.pem to generate the .pfx certificate
Openssl pkcs12 -export -in UgaBetCert.cer -inkey Ugabetp.pem -out certificate.pfx