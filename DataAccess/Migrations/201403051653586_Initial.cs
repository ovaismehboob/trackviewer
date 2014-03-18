namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrackUsers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DeviceId = c.String(),
                        Name = c.String(),
                        Email = c.String(),
                        ActivationCode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TrackUsers");
        }
    }
}
