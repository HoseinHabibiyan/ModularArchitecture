using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace  ModularArchitecture.Identity.Core
{
    public class CellToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Cell { get; private set; }
        public int Code { get; private set; }
        public string Hashed { get; private set; }

        public CellToken()
        {
            Hashed = string.IsNullOrEmpty(Cell) ? "" : Cell.GetHash();
        }

        public CellToken(string cell, int code)
        {
            Cell = cell;
            Code = code;
            Hashed = Cell.GetHash();
        }

        public void UpdateCode(int code)
        {
            Code = code;
        }
    }
}
