namespace Webvnue.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniversalPosts : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Posts", "UserId", "dbo.UserPosts");
            DropIndex("dbo.Posts", new[] { "UserId" });
            AlterColumn("dbo.Posts", "UserId", c => c.String());
            DropColumn("dbo.Posts", "OriginalPostUserId");
            DropTable("dbo.UserPosts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserPosts",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId);
            
            AddColumn("dbo.Posts", "OriginalPostUserId", c => c.String());
            AlterColumn("dbo.Posts", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Posts", "UserId");
            AddForeignKey("dbo.Posts", "UserId", "dbo.UserPosts", "UserId");
        }
    }
}
