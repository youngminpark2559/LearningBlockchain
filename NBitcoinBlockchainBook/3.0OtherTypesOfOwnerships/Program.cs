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
            RandomUtils.Random = new UnsecureRandom();

            //c Bitcoin Address is the hash of a publicKey encoded in Base58.
            var publicKeyHash = new Key().PubKey.Hash;
            var bitcoinAddress = publicKeyHash.GetAddress(Network.Main);
            Console.WriteLine(publicKeyHash); // 41e0d7ab8af1ba5452b824116a31357dc931cf28
            Console.WriteLine(bitcoinAddress); // 171LGoEKyVzgQstGwnTHVh3TFTgo5PsqiY


            //We also learned that as far as the blockchain is concerned, there is no such thing as a bitcoin address. 
            //The blockchain identifies a receiver with a ScriptPubKey, and that a ScriptPubKey could be generated from the address.

            //Create scriptPubKey from bitcoinAddress.
            var scriptPubKey = bitcoinAddress.ScriptPubKey;
            Console.WriteLine(scriptPubKey);
            //Output:
            // OP_DUP OP_HASH160 41e0d7ab8af1ba5452b824116a31357dc931cf28 OP_EQUALVERIFY OP_CHECKSIG
            //And vice versa. BitcoinAddress could be generated from a ScriptPubKey and Network.
            var sameBitcoinAddress = scriptPubKey.GetDestinationAddress(Network.Main);
            Console.WriteLine($"sameBitcoinAddress {sameBitcoinAddress}");



            //==========================================================================================
            //About P2PK

            //However, not all ScriptPubKey represents a Bitcoin Address.
            //For example, there is the first transaction in the first ever blockchain block which is called genesis block. 

            //Get genesisBlock from mainnet.
            Block genesisBlock = Network.Main.GetGenesis();
            //Get the firstTransaction from genesisBlock
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
            //Note that the form of the scriptPubKey is different from ordinary one.
            //A bitcoin address is represented by: 
            //OP_DUP OP_HASH160 <hash> OP_EQUALVERIFY OP_CHECKSIG
            //But here we have:
            //<pubkey> OP_CHECKSIG
            //In fact, at the beginning, public keys were already used directly in the ScriptPubKey.


            var firstOutputEver = firstTransactionEver.Outputs.First();
            var firstScriptPubKeyEver = firstOutputEver.ScriptPubKey;
            Console.WriteLine(firstScriptPubKeyEver);
            //04678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5f OP_CHECKSIG


            var firstBitcoinAddressEver = firstScriptPubKeyEver.GetDestinationAddress(Network.Main);
            Console.WriteLine(firstBitcoinAddressEver == null); // True

            var firstPubKeyEver = firstScriptPubKeyEver.GetDestinationPublicKeys().First();
            Console.WriteLine(firstPubKeyEver); // 04678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5f



            //Note that what P2PK(Pay To Public Key) and P2PKH(Pay To Public Key Hash) are.
            //P2PK : PrivateKey->Public Key<->ScriptPubKey.
            //P2PKH : PrivateKey->Public Key->Public Key hash<->ScriptPubKey.

            var key = new Key();
            Console.WriteLine("Pay To Public Key : " + key.PubKey.ScriptPubKey);
            Console.WriteLine("Pay To Public Key Hash : " + key.PubKey.Hash.ScriptPubKey);
            //Pay to public key : 02fb8021bc7dedcc2f89a67e75cee81fedb8e41d6bfa6769362132544dfdf072d4 OP_CHECKSIG
            //Pay to public key hash : OP_DUP OP_HASH160 0ae54d4cec828b722d8727cb70f4a6b0a88207b2 OP_EQUALVERIFY OP_CHECKSIG

            //These 2 types of payment way are referred as P2PK(Pay To Public Key) and P2PKH(Pay To Public Key Hash).
            //Satoshi later decided to use P2PKH instead of P2PK for 2 reasons.
            //1. Elliptic curve cryptography which is used by public key and private key is vulnerable to a modified Shor's algorithm for solving the discrete logarithm problem on elliptic curves.
            //It means that in the future a quantum computer might be able to retrieve a private key from a public key.
            //By publishing the public key only when the coins are spent(at this point, also assume that the addresss are not reused), such an attack is rendered ineffective.
            //2. With the hash being smaller than 20 bytes, it's easier to print and embed into small storage mediums like QR codes.
            //Nowadays, there's no reason to use P2PK directly although it's still used in combination with P2SH. P2SH is talked more in detail in later chapter.
            //If the issue of the early use of P2Pk is not addressed, it will have a serious impact on the Bitcoin price.



            //============================================================================================

            //P2WPKH(Pay to Witness Public Key Hash)
            //In 2015, Pieter Wuille introduced a new feature to bitcoin called Segregated Witness, also known by it's abbreviated name, Segwit. Basically, Segregated Witness moves the proof of ownership from the scriptSig part of the transaction to a new part called the witness of the input.
            //There are several reasons why it is beneficial to use this new scheme, a summary of which are presented below.For more details visit https://bitcoincore.org/en/2016/01/26/segwit-benefits/.
            //1.Third party Malleability Fix: Previously, a third party could change the transaction id of your transaction before it was confirmed.This can not occur under Segwit.
            //2.Linear sig hash scaling: Signing a transaction used to require hashing the whole transaction for every input. This was a potential DDoS vector attack for large transactions.
            //3.Signing of input values: The amount that is spent in an input is also signed, meaning that the signer can’t be tricked about the amount of fees that are actually being paid.
            //4.Capacity increase: It will now be possible to have more than 1MB of transactions in each block(which are created every 10 minutes on average).Segwit increases this capacity by a factor of about 2.1, based upon the average transaction profile from November 2016.
            //5.Fraud proof: Will be developed later, but SPV wallets will be able to validate more consensus rules rather than just simply following the longest chain.

            //Before Sewgit, in the transaction, signature was used in the calculation of the transaction id.
            //At that time, signature was located in TxIn, but after Segwi
            //The signature contains the same information as a P2PKH spend, but signature is located in the witness instead of the scriptSig. The scriptPubKey though, is modified from
            //OP_DUP OP_HASH160 0067c8970e65107ffbb436a49edd8cb8eb6b567f OP_EQUALVERIFY OP_CHECKSIG
            //To
            //0 0067c8970e65107ffbb436a49edd8cb8eb6b567f



            //For nodes which did not upgrade, this looks like two pushes on the stack. This means that any scriptSig can spend them. So even without the signatures, old nodes will consider such transactions valid. New nodes interpret the first push as the witness version and the second push as the witness program.
            //New nodes will therefore also require the signature in order to verify the transaction.
            //In NBitcoin, spending a P2WPKH output is no different from spending a normal P2PKH.
            //To get the ScriptPubKey from a public key simply use PubKey.WitHash instead of PubKey.Hash.
            var key1 = new Key();
            Console.WriteLine(key1.PubKey.WitHash.ScriptPubKey);
            //Which will output something like
            //0 0067c8970e65107ffbb436a49edd8cb8eb6b567f


            //Signing the spending of such coins will be explained later in the “Using the TransactionBuilder" section, and does not differ in any way from the code used to sign a P2PKH output.
            //The witness data is similar to the scriptSig of P2PKH, and the scriptSig data is empty:

            // "in": [
            // {
            // "prev_out": 
            // {
            // "hash": "725497eaef527567a0a18b310bbdd8300abe86f82153a39d2f87fef713dc8177",
            // "n": 0
            // },
            // "scriptSig": "",
            // "witness": "3044022079d443be2bd39327f92adf47a34e4b6ad7c82af182c71fe76ccd39743ced58cf0220149de3e8f11e47a989483f371d3799a710a7e862dd33c9bd842c417002a1c32901 0363f24cd2cb27bb35eb2292789ce4244d55ce580218fd81688197d4ec3b005a67"
            // }

            //Once again, the semantics of P2WPKH is the same as the semantics of P2PKH, except that the signature is not placed at the same location as before.




            //=========================================================================================
            //MUSTISIG 

            //It is possible to have shared ownership and control over coins.
            //In order to demonstrate this, we will create a ScriptPubKey that represents an m - of - n multi sig. 
            //This means that in order to spend the coins, m number of private keys will need to sign the spending transaction out of the n number of different public keys provided.
            //Let’s create a multi sig with Bob, Alice and Satoshi, where two of the three of them need to sign a transaction in order to spend a coin.


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
            //Output:
            //2 0282213c7172e9dff8a852b436a957c1f55aa1a947f2571585870bfb12c0c15d61 036e9f73ca6929dec6926d8e319506cc4370914cd13d300e83fd9c3dfca3970efb 0324b9185ec3db2f209b620657ce0e9a792472d89911e0ac3fc1e5b5fc2ca7683d 3 OP_CHECKMULTISIG
            //As you can see, the scriptPubkey has the following form: 
            //<sigsRequired> <pubkeys…> <pubKeysCount> OP_CHECKMULTISIG

            //The process for signing, it is a little more complicated than just calling Transaction.Sign, which does not work for multi sig.
            //Later, we will talk more deeply about the subject but for now let’s use the TransactionBuilder for signing the transaction.

            //Imagine the multi-sig scriptPubKey received a coin in a transaction called received:
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


            //========================================================================================
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
