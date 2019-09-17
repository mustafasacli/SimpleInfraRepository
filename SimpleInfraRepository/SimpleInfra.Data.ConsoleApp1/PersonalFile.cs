namespace SimpleInfra.Data.ConsoleApp1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PersonalFile")]
    public partial class PersonalFile
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FileName { get; set; }

        [Required]
        public byte[] FileContent { get; set; }

        public int CreatedBy { get; set; }

        //[Column(TypeName = "datetime2")]
        public DateTime CreationDate { get; set; }

        public bool IsActive { get; set; }

        [NotMapped]
        public string CreationDateText
        { get { return this.CreationDate.ToString("yyyy-MM-dd HH:mm:ss.ffffff"); } }

        public override string ToString()
        {
            return $"Id: {this.Id},{Environment.NewLine}FileName: {this.FileName},{Environment.NewLine}CreatedBy: {this.CreatedBy},{Environment.NewLine}CreationDate: {this.CreationDateText},{Environment.NewLine}IsActive: {IsActive}";
        }
    }

    [Table("PERSONAL_FILE")]
    public partial class PersonalFile2
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("FILE_NAME")]
        public string FileName { get; set; }

        [Required]
        [Column("FILE_CONTENT")]
        public byte[] FileContent { get; set; }

        [Column("CREATED_BY")]
        public long CreatedBy { get; set; }

        [Column("CREATION_DATE")]
        public DateTime CreationDate { get; set; }

        [NotMapped]
        public string CreationDateText
        { get { return this.CreationDate.ToString("yyyy-MM-dd hh:mm:ss.ffffff"); } }

        [Column("IS_ACTIVE")]
        public long IsActive { get; set; }

        public override string ToString()
        {
            return $"Id: {this.Id},{Environment.NewLine}FileName: {this.FileName},{Environment.NewLine}CreatedBy: {this.CreatedBy},{Environment.NewLine}CreationDate: {this.CreationDateText},{Environment.NewLine}IsActive: {IsActive}";
        }
    }
}
