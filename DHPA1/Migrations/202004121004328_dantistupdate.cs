namespace DHPA1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dantistupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dantists", "Salt", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dantists", "Salt");
        }
    }
}
