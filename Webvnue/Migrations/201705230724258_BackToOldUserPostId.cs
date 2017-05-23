namespace Webvnue.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BackToOldUserPostId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Posts", "OriginalPostUserId_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Posts", new[] { "OriginalPostUserId_Id" });
            AddColumn("dbo.Posts", "OriginalPostUserId", c => c.String());
            DropColumn("dbo.Posts", "OriginalPostUserId_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Posts", "OriginalPostUserId_Id", c => c.String(maxLength: 128));
            DropColumn("dbo.Posts", "OriginalPostUserId");
            CreateIndex("dbo.Posts", "OriginalPostUserId_Id");
            AddForeignKey("dbo.Posts", "OriginalPostUserId_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
