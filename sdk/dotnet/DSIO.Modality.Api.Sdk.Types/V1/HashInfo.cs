namespace DS.IO.ModalityApi.V1.Types
{
    public class HashInfo
    {
        /// <summary>
        /// Algorithm used to compute the hash, i.e., md5
        /// </summary>
        public string Alg { get; set; }

        /// <summary>
        /// The hash value
        /// </summary>
        public string Hash { get; set; }
    }
}
