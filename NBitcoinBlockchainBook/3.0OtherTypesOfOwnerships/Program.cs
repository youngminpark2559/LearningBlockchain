using System;
using System.Linq;
using System.Text;
using NBitcoin;
using NBitcoin.Crypto;

namespace _3._0OtherTypesOfOwnerships
{
    class Program
    {
        static void Main()
        {
            //c Bitcoin Address is the hash of a public key(K).
            //??
            var publicKeyHash = new Key().PubKey.Hash;
            var bitcoinAddress = publicKeyHash.GetAddress(Network.Main);
            Console.WriteLine(publicKeyHash); // 41e0d7ab8af1ba5452b824116a31357dc931cf28
            Console.WriteLine(bitcoinAddress); // 171LGoEKyVzgQstGwnTHVh3TFTgo5PsqiY

            //The blockchain identifies a receiver with a ScriptPubKey.
            //A ScriptPubKey could be generated from the address.
            var scriptPubKey = bitcoinAddress.ScriptPubKey;
            Console.WriteLine(scriptPubKey); // OP_DUP OP_HASH160 41e0d7ab8af1ba5452b824116a31357dc931cf28 OP_EQUALVERIFY OP_CHECKSIG
            //And vice versa. Address could be generated from a ScriptPubKey.
            var sameBitcoinAddress = scriptPubKey.GetDestinationAddress(Network.Main);



            //However, not all ScriptPubKey represents a Bitcoin Address.
            //For example, there is the first transaction in the first ever blockchain block which is called genesis block. 
            Block genesisBlock = Network.Main.GetGenesis();
            Transaction firstTransactionEver = genesisBlock.Transactions.First();
            Console.WriteLine(firstTransactionEver);
            //{
            //…
            //  "out": [
            //    {
            //      "value": "50.00000000",
            //      "scriptPubKey": "04678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5f OP_CHECKSIG"
            //    }
            //  ]
            //}

            var firstOutputEver = firstTransactionEver.Outputs.First();
            var firstScriptPubKeyEver = firstOutputEver.ScriptPubKey;
            Console.WriteLine(firstScriptPubKeyEver); 
            //04678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5f OP_CHECKSIG


            var firstBitcoinAddressEver = firstScriptPubKeyEver.GetDestinationAddress(Network.Main);
            Console.WriteLine(firstBitcoinAddressEver == null); // True

            var firstPubKeyEver = firstScriptPubKeyEver.GetDestinationPublicKeys().First();
            Console.WriteLine(firstPubKeyEver); // 04678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5f




            var key = new Key();
            //Private Key->Public Key<->ScriptPubKey
            Console.WriteLine("Pay to public key : " + key.PubKey.ScriptPubKey);
            //Private Key->Public Key->Public Key Hash<->ScriptPubKey
            Console.WriteLine("Pay to public key hash : " + key.PubKey.Hash.ScriptPubKey);
            //Pay to public key : 02fb8021bc7dedcc2f89a67e75cee81fedb8e41d6bfa6769362132544dfdf072d4 OP_CHECKSIG
            //Pay to public key hash : OP_DUP OP_HASH160 0ae54d4cec828b722d8727cb70f4a6b0a88207b2 OP_EQUALVERIFY OP_CHECKSIG

            //These 2 types of payment way are referred as P2PK(Pay To Public Key) and P2PKH(Pay To Public Key Hash).
            //Satoshi later decided to use P2PKH instead of P2PK for 2 reasons.
            //1. Elliptic curve cryptography which is used by public key and private key is vulnerable to a modified Shor's algorithm for solving the discrete logarithm problem on elliptic curves.
            //It means that in the future a quantum computer might be able to retrieve a private key from a public key.
            //By publishing the public key only when the coins are spent(at this point, also assume that the addresss are not reused), such an attack is rendered ineffective.
            //2. With the hash being small than 20 bytes, it's easier to print and embed into small storage meidums like QR codes.
            //Nowadays, there's no reason to use P2PK directly although it's still used in combination with P2SH.P2SH is more in detail later chapter.
            //Let's think and discuss, if the issue of the early use of P2Pk is not addressed, it will have a serious impact on the Bitcoin price.



            /* MUSTISIG */

            Key bob = new Key();
            Key alice = new Key();
            Key satoshi = new Key();

            scriptPubKey = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, new PubKey[]
            {
                bob.PubKey,
                alice.PubKey,
                satoshi.PubKey
            });

            Console.WriteLine(scriptPubKey);

            var received = new Transaction();
            received.Outputs.Add(new TxOut(Money.Coins(1.0m), scriptPubKey));

            Coin coin = received.Outputs.AsCoins().First();

            BitcoinAddress nico = new Key().PubKey.GetAddress(Network.Main);
            TransactionBuilder builder = new TransactionBuilder();
            Transaction unsigned =
                builder
                    .AddCoins(coin)
                    .Send(nico, Money.Coins(1.0m))
                    .BuildTransaction(sign: false);

            Transaction aliceSigned =
                builder
                    .AddCoins(coin)
                    .AddKeys(alice)
                    .SignTransaction(unsigned);

            Transaction bobSigned =
                builder
                    .AddCoins(coin)
                    .AddKeys(bob)
                    .SignTransaction(unsigned);

            Transaction fullySigned =
                builder
                    .AddCoins(coin)
                    .CombineSignatures(aliceSigned, bobSigned);

            Console.WriteLine(fullySigned);

            /* Pay to Script Hash */

            Console.WriteLine(scriptPubKey);
            Console.WriteLine(scriptPubKey.Hash.ScriptPubKey);

            Script redeemScript =
                PayToMultiSigTemplate
                .Instance
                .GenerateScriptPubKey(2, new[] { bob.PubKey, alice.PubKey, satoshi.PubKey });
            received = new Transaction();
            //Pay to the script hash
            received.Outputs.Add(new TxOut(Money.Coins(1.0m), redeemScript.Hash));

            ScriptCoin scriptCoin = received.Outputs.AsCoins().First().ToScriptCoin(redeemScript);

            // P2SH(P2WPKH)

            Console.WriteLine(key.PubKey.WitHash.ScriptPubKey.Hash.ScriptPubKey);

            Console.WriteLine(key.PubKey.ScriptPubKey.WitHash.ScriptPubKey.Hash.ScriptPubKey);

            // Arbitrary

            BitcoinAddress address = BitcoinAddress.Create("1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB");
            var birth = Encoding.UTF8.GetBytes("18/07/1988");
            var birthHash = Hashes.Hash256(birth);
            redeemScript = new Script(
                "OP_IF "
                    + "OP_HASH256 " + Op.GetPushOp(birthHash.ToBytes()) + " OP_EQUAL " +
                "OP_ELSE "
                    + address.ScriptPubKey + " " +
                "OP_ENDIF");

            var tx = new Transaction();
            tx.Outputs.Add(new TxOut(Money.Parse("0.0001"), redeemScript.Hash));
            scriptCoin = tx.Outputs.AsCoins().First().ToScriptCoin(redeemScript);

            //Create spending transaction
            Transaction spending = new Transaction();
            spending.AddInput(new TxIn(new OutPoint(tx, 0)));

            ////Option 1 : Spender knows my birthdate
            Op pushBirthdate = Op.GetPushOp(birth);
            Op selectIf = OpcodeType.OP_1; //go to if
            Op redeemBytes = Op.GetPushOp(redeemScript.ToBytes());
            Script scriptSig = new Script(pushBirthdate, selectIf, redeemBytes);
            spending.Inputs[0].ScriptSig = scriptSig;

            //Verify the script pass
            var result = spending
                            .Inputs
                            .AsIndexedInputs()
                            .First()
                            .VerifyScript(tx.Outputs[0].ScriptPubKey);
            Console.WriteLine(result); // True
                                       ///////////

            ////Option 2 : Spender knows my private key
            BitcoinSecret secret = new BitcoinSecret("...");
            var sig = spending.SignInput(secret, scriptCoin);
            var p2pkhProof = PayToPubkeyHashTemplate
                .Instance
                .GenerateScriptSig(sig, secret.PrivateKey.PubKey);
            selectIf = OpcodeType.OP_0; //go to else
            scriptSig = p2pkhProof + selectIf + redeemBytes;
            spending.Inputs[0].ScriptSig = scriptSig;


            //Verify the script pass
            result = spending
                            .Inputs
                            .AsIndexedInputs()
                            .First()
                            .VerifyScript(tx.Outputs[0].ScriptPubKey);
            Console.WriteLine(result);


            Console.ReadLine();
        }
    }
}
