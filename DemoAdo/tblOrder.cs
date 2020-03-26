namespace DemoAdo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblOrder")]
    public partial class tblOrder
    {
        [Key]
        public int FOrderID { get; set; }

        [Required]
        [StringLength(10)]
        public string FOrderNum { get; set; }

        public int FClientID { get; set; }

        public int? FCustID { get; set; }

        [StringLength(255)]
        public string FOrderCust { get; set; }

        public int FOrderQty { get; set; }

        public short? FInputQty { get; set; }

        public short? FShipQty { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public byte? FModelQty { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FDeliveryDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FPlanDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FStartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FProduceDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FFinishDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FCancelDate { get; set; }

        [StringLength(255)]
        public string FNote { get; set; }

        [Required]
        [StringLength(4)]
        public string FOrderType { get; set; }

        public bool FUrgent { get; set; }

        [StringLength(5)]
        public string FCurrency { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [StringLength(5)]
        public string FStatus { get; set; }

        public bool FPrintInfo { get; set; }

        [StringLength(25)]
        public string FSortCode { get; set; }

        public int? FCustPrintingInfoID { get; set; }

        public short? FQtyYield { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public short? FQtyYieldBalance { get; set; }

        [Column(TypeName = "money")]
        public decimal? FShipAmount { get; set; }

        [Column(TypeName = "money")]
        public decimal? FCostAmount { get; set; }

        [Column(TypeName = "money")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? FAmountBalance { get; set; }

        public bool FMould { get; set; }

        public int? FPackingTemplateID { get; set; }

        public bool FPacking { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FBillDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public short? FQtyBalance { get; set; }

        [Required]
        [StringLength(10)]
        public string FOrderStatus { get; set; }
    }
}
