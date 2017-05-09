namespace Webvnue.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserFollows : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Follows",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(),
                        FollowingUserId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Follows");
        }
    }
}
