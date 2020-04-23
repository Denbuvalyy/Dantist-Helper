namespace DHPA1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class toothSimple : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teeth", "Position", c => c.Int(nullable: false));
            AddColumn("dbo.Teeth", "Description", c => c.String());
            DropColumn("dbo.Teeth", "Name_Number");
            DropColumn("dbo.Teeth", "Name_Left");
            DropColumn("dbo.Teeth", "Name_Top");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Teeth", "Name_Top", c => c.Boolean(nullable: false));
            AddColumn("dbo.Teeth", "Name_Left", c => c.Boolean(nullable: false));
            AddColumn("dbo.Teeth", "Name_Number", c => c.Int(nullable: false));
            DropColumn("dbo.Teeth", "Description");
            DropColumn("dbo.Teeth", "Position");
        }
    }
}
