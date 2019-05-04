namespace SimpleInfra.Data.ConsoleApp1
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PERSONAL_FILE")]
    public partial class PersonalFile
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