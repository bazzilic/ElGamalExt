﻿/************************************************************************************
 This implementation of the ElGamal encryption scheme is based on the code from [1].

 This library is provided as-is and is covered by the MIT License [2] (except for the
 parts that belong to O'Reilly - they are covered by [3]).

 [1] Adam Freeman & Allen Jones, Programming .NET Security: O'Reilly Media, 2003,
     ISBN 9780596552275 (http://books.google.com.sg/books?id=ykXCNVOIEuQC)

 [2] The MIT License (MIT), website, (http://opensource.org/licenses/MIT)

 [3] Tim O'Reilly, O'Reilly Policy on Re-Use of Code Examples from Books: website,
     2001, (http://www.oreillynet.com/pub/a/oreilly/ask_tim/2001/codepolicy.html)
 ************************************************************************************/

using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.Numerics;

namespace ElGamalExt
{
    public enum ElGamalPaddingMode : byte
    {
        ANSIX923,
        LeadingZeros,
        TrailingZeros,
        BigIntegerPadding
    }

    public abstract class ElGamal : AsymmetricAlgorithm
    {
        public ElGamalPaddingMode Padding;

        public abstract void ImportParameters(ElGamalParameters p_parameters);
        public abstract ElGamalParameters ExportParameters(bool p_include_private_params);
        public abstract byte[] EncryptBigInteger(BigInteger p_data);
        public abstract BigInteger DecryptBigInteger(byte[] p_data);
        public abstract byte[] Sign(byte[] p_hashcode);
        public abstract bool VerifySignature(byte[] p_hashcode, byte[] p_signature);

        public abstract byte[] Multiply(byte[] p_first, byte[] p_second);

        public override string ToXmlString(bool p_include_private)
        {
            var x_params = ExportParameters(p_include_private);

            var x_sb = new StringBuilder();

            x_sb.Append("<ElGamalKeyValue>");

            x_sb.Append("<P>" + Convert.ToBase64String(x_params.P) + "</P>");
            x_sb.Append("<G>" + Convert.ToBase64String(x_params.G) + "</G>");
            x_sb.Append("<Y>" + Convert.ToBase64String(x_params.Y) + "</Y>");
            x_sb.Append("<Padding>" + x_params.Padding.ToString() + "</Padding>");

            if (p_include_private)
            {
                // we need to include X, which is the part of private key
                x_sb.Append("<X>" + Convert.ToBase64String(x_params.X) + "</X>");
            }

            x_sb.Append("</ElGamalKeyValue>");

            return x_sb.ToString();
        }

        public override void FromXmlString(string p_string)
        {
            var x_params = new ElGamalParameters();

            var keyValues = XDocument.Parse(p_string).Element("ElGamalKeyValue");

            x_params.P = Convert.FromBase64String((String)keyValues.Element("P") ?? "");
            x_params.G = Convert.FromBase64String((String)keyValues.Element("G") ?? "");
            x_params.Y = Convert.FromBase64String((String)keyValues.Element("Y") ?? "");
            x_params.Padding = (ElGamalPaddingMode)Enum.Parse(typeof(ElGamalPaddingMode), (String)keyValues.Element("Padding") ?? "");
            x_params.X = Convert.FromBase64String((String)keyValues.Element("X") ?? "");

            ImportParameters(x_params);
        }
    }
}
