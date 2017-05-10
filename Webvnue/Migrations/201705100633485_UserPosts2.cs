namespace Webvnue.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserPosts2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "OriginalPostUserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "OriginalPostUserId");
        }
    }
}
