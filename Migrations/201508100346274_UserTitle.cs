namespace Lcps.Division.Directory.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserTitle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Title", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Title");
        }
    }
}
