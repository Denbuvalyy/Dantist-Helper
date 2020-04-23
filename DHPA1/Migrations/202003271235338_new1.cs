namespace DHPA1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class new1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dantists",
                c => new
                    {
                        DantistId = c.Int(nullable: false, identity: true),
                        Surname = c.String(),
                        Name = c.String(),
                        DantistAddress = c.String(),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                        WorkPlace_Private = c.Boolean(nullable: false),
                        WorkPlace_PlaceName = c.String(),
                        WorkPlace_City = c.String(),
                        WorkPlace_HouseNumber = c.Int(nullable: false),
                        WorkPlace_StreetName = c.String(),
                        DoB = c.DateTime(nullable: false),
                        Login = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.DantistId);
            
            CreateTable(
                "dbo.Patients",
                c => new
                    {
                        PatientId = c.Int(nullable: false, identity: true),
                        Surname = c.String(),
                        Name = c.String(),
                        DOB = c.DateTime(nullable: false),
                        WorkPlace = c.String(),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                        Address_StreetName = c.String(),
                        Address_CityOrTown = c.String(),
                        Address_Region = c.String(),
                        Address_Country = c.String(),
                        Address_PostIndex = c.Int(nullable: false),
                        Warnings = c.String(),
                        LastVisitDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PatientId);
            
            CreateTable(
                "dbo.Teeth",
                c => new
                    {
                        ToothId = c.Int(nullable: false, identity: true),
                        Name_Number = c.Int(nullable: false),
                        Name_Left = c.Boolean(nullable: false),
                        Name_Top = c.Boolean(nullable: false),
                        Manipulated = c.Boolean(nullable: false),
                        Patient_PatientId = c.Int(),
                    })
                .PrimaryKey(t => t.ToothId)
                .ForeignKey("dbo.Patients", t => t.Patient_PatientId)
                .Index(t => t.Patient_PatientId);
            
            CreateTable(
                "dbo.Manipulations",
                c => new
                    {
                        ManipulationId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        ManipulationDate = c.DateTime(nullable: false),
                        Tooth_ToothId = c.Int(),
                    })
                .PrimaryKey(t => t.ManipulationId)
                .ForeignKey("dbo.Teeth", t => t.Tooth_ToothId)
                .Index(t => t.Tooth_ToothId);
            
            CreateTable(
                "dbo.Pictures",
                c => new
                    {
                        PictureId = c.Int(nullable: false, identity: true),
                        Caption = c.String(),
                        PicturePath = c.String(),
                        VisitId = c.Int(),
                        ManipulationId = c.Int(),
                    })
                .PrimaryKey(t => t.PictureId)
                .ForeignKey("dbo.Manipulations", t => t.ManipulationId)
                .ForeignKey("dbo.Visits", t => t.VisitId)
                .Index(t => t.VisitId)
                .Index(t => t.ManipulationId);
            
            CreateTable(
                "dbo.Visits",
                c => new
                    {
                        VisitId = c.Int(nullable: false, identity: true),
                        VisitDate = c.DateTime(nullable: false),
                        Description = c.String(),
                        Patient_PatientId = c.Int(),
                    })
                .PrimaryKey(t => t.VisitId)
                .ForeignKey("dbo.Patients", t => t.Patient_PatientId)
                .Index(t => t.Patient_PatientId);
            
            CreateTable(
                "dbo.PatientDantists",
                c => new
                    {
                        Patient_PatientId = c.Int(nullable: false),
                        Dantist_DantistId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Patient_PatientId, t.Dantist_DantistId })
                .ForeignKey("dbo.Patients", t => t.Patient_PatientId, cascadeDelete: true)
                .ForeignKey("dbo.Dantists", t => t.Dantist_DantistId, cascadeDelete: true)
                .Index(t => t.Patient_PatientId)
                .Index(t => t.Dantist_DantistId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Visits", "Patient_PatientId", "dbo.Patients");
            DropForeignKey("dbo.Teeth", "Patient_PatientId", "dbo.Patients");
            DropForeignKey("dbo.Manipulations", "Tooth_ToothId", "dbo.Teeth");
            DropForeignKey("dbo.Pictures", "VisitId", "dbo.Visits");
            DropForeignKey("dbo.Pictures", "ManipulationId", "dbo.Manipulations");
            DropForeignKey("dbo.PatientDantists", "Dantist_DantistId", "dbo.Dantists");
            DropForeignKey("dbo.PatientDantists", "Patient_PatientId", "dbo.Patients");
            DropIndex("dbo.PatientDantists", new[] { "Dantist_DantistId" });
            DropIndex("dbo.PatientDantists", new[] { "Patient_PatientId" });
            DropIndex("dbo.Visits", new[] { "Patient_PatientId" });
            DropIndex("dbo.Pictures", new[] { "ManipulationId" });
            DropIndex("dbo.Pictures", new[] { "VisitId" });
            DropIndex("dbo.Manipulations", new[] { "Tooth_ToothId" });
            DropIndex("dbo.Teeth", new[] { "Patient_PatientId" });
            DropTable("dbo.PatientDantists");
            DropTable("dbo.Visits");
            DropTable("dbo.Pictures");
            DropTable("dbo.Manipulations");
            DropTable("dbo.Teeth");
            DropTable("dbo.Patients");
            DropTable("dbo.Dantists");
        }
    }
}
