namespace Webvnue.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserProfileBio : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfileBios",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserID = c.String(),
                        AboutMe = c.String(),
                        Location = c.String(),
                        Gender = c.String(),
                        Quote = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserProfileBios");
        }
    }
}
