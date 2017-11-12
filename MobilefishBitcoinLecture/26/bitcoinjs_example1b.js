// Generate 10 Testnet3 Bitcoin private keys and its corresponding public addresses.
// Store these data in a comma separated file.
//
// More information:
// Example 1b
// https://www.mobilefish.com/developer/nodejs/nodejs_quickguide_bitcoinjs.html
//
// Disclaimer:
//
// Use this script at your own risk! All information on this page is provided "as is", without any warranty.
// Mobilefish.com will not be liable for any damages, loss of profits or any other kind of loss
// you may sustain by using this script.

var bitcoin = require('bitcoinjs-lib'); // Use version 2.2.0
var fs = require('fs');

// Generate number of private keys
var maxKeys = 10;

// Name of the output file
var outputFile = "private_keys_testnet.txt";

var stream = fs.createWriteStream(outputFile);

stream.once('open', function(fd) {
	for (var i=0; i<maxKeys; i++) {
		var keyPair = bitcoin.ECPair.makeRandom({network: bitcoin.networks.testnet});
		var privateKeyWIFCompressed = keyPair.toWIF();
		var publicKeyCompressed = keyPair.getAddress();
		var line = [i, privateKeyWIFCompressed, publicKeyCompressed];
		stream.write(line.join(", "));
		stream.write("\n");
	}
	stream.end();
});