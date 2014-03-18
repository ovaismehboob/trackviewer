namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsActivatedBit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrackUsers", "IsActivated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrackUsers", "IsActivated");
        }
    }
}
