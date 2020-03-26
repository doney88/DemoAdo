namespace DemoAdo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblOrderSub")]
    public partial class tblOrderSub
    {
        [Key]
        public int FSKUID { get; set; }

        public int FOrderID { get; set; }

        public byte? FSerial { get; set; }

        public int FMaterialID { get; set; }

        [StringLength(255)]
        public string FModelCust { get; set; }

        [Required]
        [StringLength(50)]
        public string FColorCust { get; set; }

        [StringLength(9)]
        public string FSizePrint { get; set; }

        public short FQty { get; set; }

        public short? FQtyInput { get; set; }

        public int? FQtyYield { get; set; }

        public short FQtyShip { get; set; }

        [Column(TypeName = "money")]
        public decimal? FPrice { get; set; }

        [StringLength(255)]
        public string FNote { get; set; }

        [Column(TypeName = "money")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? FAmount { get; set; }

        public bool FVAT { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? FBalancePercent { get; set; }

        [Column(TypeName = "money")]
        public decimal? FMaterialAmount { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? FQtyBalance { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? FQtyYieldBalance { get; set; }

        [StringLength(255)]
        public string FModelPrint { get; set; }

        public bool FFinishSKU { get; set; }
    }
}
