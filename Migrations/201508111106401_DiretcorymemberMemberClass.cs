namespace Lcps.Division.Directory.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DiretcorymemberMemberClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "MemberClass", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "MemberClass");
        }
    }
}
