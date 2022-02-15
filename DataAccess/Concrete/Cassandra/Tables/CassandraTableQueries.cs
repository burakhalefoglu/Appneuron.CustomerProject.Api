namespace DataAccess.Concrete.Cassandra.Tables
{
    public static class CassandraTableQueries
    {
        public static string AppneuronProduct => "CREATE TABLE IF NOT EXISTS ClientProjectsDatabase.appneuron_products(id bigint, product_name text, status boolean,  PRIMARY KEY(id))";
        public static string Client => "CREATE TABLE IF NOT EXISTS ClientProjectsDatabase.clients(id bigint, client_id bigint, project_id bigint, created_at date, is_paid_client boolean,  status boolean,  PRIMARY KEY(id))";
        public static string Customer => "CREATE TABLE IF NOT EXISTS ClientProjectsDatabasecustomers(id bigint, customer_scale_id bigint, demographic_id bigint, industry_id bigint, status boolean,  PRIMARY KEY(id))";
        public static string CustomerDemographic => "CREATE TABLE IF NOT EXISTS ClientProjectsDatabase.customer_demographics(id bigint, customer_desc text, status boolean,  PRIMARY KEY(id))";
        public static string CustomerDiscount => "CREATE TABLE IF NOT EXISTS ClientProjectsDatabase.customer_discounts(id bigint, user_id bigint, discount_id bigint, status boolean,  PRIMARY KEY(id))";
        public static string CustomerProject => "CREATE TABLE IF NOT EXISTS ClientProjectsDatabase.customer_projects(id bigint, customer_id bigint, vote_id bigint, created_at date, project_name text, project_body text,   status boolean,  PRIMARY KEY(id))";
        public static string CustomerProjectHasProduct => "CREATE TABLE IF NOT EXISTS ClientProjectsDatabase.customer_project_has_products(id bigint, product_id bigint, project_id bigint,  status boolean,  PRIMARY KEY(id))";
        public static string CustomerScale => "CREATE TABLE IF NOT EXISTS ClientProjectsDatabase.customer_scales(id bigint, name text, description text, status boolean,  PRIMARY KEY(id))";
        public static string Discount => "CREATE TABLE IF NOT EXISTS ClientProjectsDatabase.discounts(id bigint, DiscountName text, percent tinyint, status boolean,  PRIMARY KEY(id))";
        public static string GamePlatform => "CREATE TABLE IF NOT EXISTS ClientProjectsDatabase.game_platforms(id bigint, platform_name text, platform_description text, status boolean,  PRIMARY KEY(id))";
        public static string Industry => "CREATE TABLE IF NOT EXISTS ClientProjectsDatabase.industries(id bigint, name text, status boolean,  PRIMARY KEY(id))";
        public static string Invoice => "CREATE TABLE IF NOT EXISTS ClientProjectsDatabase.invoices(id bigint, bill_no text, created_at date, last_payment_time date, user_id bigint, discount_id bigint, unit_price int, is_it_paid boolean,  status boolean,  PRIMARY KEY(id))";
        public static string Vote => "CREATE TABLE IF NOT EXISTS ClientProjectsDatabase.votes(id bigint, vote_name text, vote_value tinyint, status boolean,  PRIMARY KEY(id))";
        public static string Log => "CREATE TABLE IF NOT EXISTS ClientProjectsDatabase.logs(id bigint, message_template text, level text, time_stamp date,  exception text, status boolean,  PRIMARY KEY(id))";
    }
}