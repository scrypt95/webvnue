namespace Webvnue.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PostCommentsNow : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        OriginalUserId = c.String(),
                        Message = c.String(),
                        TimeStamp = c.DateTime(nullable: false),
                        Post_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.Post_Id)
                .Index(t => t.Post_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "Post_Id", "dbo.Posts");
            DropIndex("dbo.Comments", new[] { "Post_Id" });
            DropTable("dbo.Comments");
        }
    }
}
