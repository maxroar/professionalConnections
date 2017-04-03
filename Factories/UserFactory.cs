using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Options;
using csbb0328.Models;
using Dapper;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace csbb0328.Factory
{
    public class UserFactory : IFactory<User> {
    private readonly IOptions<MySqlOptions> mysqlConfig;
    
    public UserFactory(IOptions<MySqlOptions> conf) {
            mysqlConfig = conf;
    }
    
    internal IDbConnection Connection {
      get {
         return new MySqlConnection(mysqlConfig.Value.ConnectionString);
      }
    }

    public void AddUser(User user)
    {
      using(IDbConnection dbConnection = Connection){
        string query = "INSERT into users (name, email, description, password) VALUES (@name, @email, @description, @password)";
        dbConnection.Open();
        dbConnection.Execute(query, user);

      }
      
    }
    public User GetCurrentUser(string email)
    {
      using(IDbConnection dbConnection = Connection){
        string query = $"SELECT * FROM users WHERE email = '{email}' LIMIT 1";
        dbConnection.Open();
        User current = dbConnection.QuerySingleOrDefault<User>(query);
        System.Console.WriteLine(current);
        return current;
      }
    }

    public User GetUserByID(int id)
    {
      using(IDbConnection dbConnection = Connection){
        string query = $"SELECT * FROM users WHERE id = {id} LIMIT 1";
        dbConnection.Open();
        User current = dbConnection.QuerySingleOrDefault<User>(query);
        System.Console.WriteLine(current);
        return current;
      }
    }

    public bool CheckUserInDB(string email){
      User newUser = GetCurrentUser(email);
      return newUser != null;
    }

    public bool CheckLogin(User user){
      User newUser = GetCurrentUser(user.email);
      var hasher = new PasswordHasher<User>();
      if(0 != hasher.VerifyHashedPassword(user, user.password, newUser.password)){
        return true;
      }
      return false;
    }

    public List<User> GetConnectedUsers(int uid)
    {
        List<User> connectedUsers = new List<User>();
        using(IDbConnection dbConnection = Connection)
        {
            string query = $"select users.id, users.name from connections left join users on connections.userId1 = users.id where connections.userId2 = {uid} and connections.accepted = true";
            dbConnection.Open();
            connectedUsers = dbConnection.Query<User>(query).ToList();
            return connectedUsers;
        }
    }

    public List<User> GetRequestedUsers(int uid)
    {
        List<User> requestedUsers = new List<User>();
        using(IDbConnection dbConnection = Connection)
        {
            string query = $"select users.id, users.name from connections left join users on connections.userId1 = users.id where connections.userId2 = {uid} and connections.accepted = false";
            dbConnection.Open();
            requestedUsers = dbConnection.Query<User>(query).ToList();
            System.Console.WriteLine(requestedUsers);
            return requestedUsers;
        }
    }

    public List<User> GetAllConnectUsers(int uid)
    {
        List<User> otherUsers = new List<User>();
        using(IDbConnection dbConnection = Connection)
        {
            // string query = $"select users.id, users.name, usersc.id, usersc.name from users join connections on {uid} = connections.userId1 or {uid} = connections.userId2 join users usersc on (users.id = connections.userId1 and usersc.id <> users.id and connections.accepted = 0) or (usersc.id = connections.userId2 and usersc.id <> users.id and connections.accepted = 0)";
            string query = $"select users.id, users.name from connections left join users on connections.userId2 = users.id where connections.userId1 = {uid}";
            dbConnection.Open();
            otherUsers = dbConnection.Query<User>(query).ToList();
            return otherUsers;
        }
    }

    public List<User> GetAllUsers()
    {
        using(IDbConnection dbConnection = Connection)
        {
            // List<User> allUsers = new List<User>();
            string query = $"select * from users";
            dbConnection.Open();
            // using (GridReader multi = dbConnection.QueryMultiple(query, null))
            // {
            //     List<User> allUsers = multi.Read<Post, User, Post>((post, user) => {post.user = user; return post;}, splitOn: "user_id").ToList();
            //     List<Comment> Comments = multi.Read<Comment, User, Comment>((comment, user) => {comment.user = user; return comment;}, splitOn: "user_id").ToList();
            //     List<Post> combo = Posts.GroupJoin(
            //         Comments,
            //         post => post.id,
            //         comment => comment.post_id,
            //         (post, comments) =>
            //         {
            //         post.comments = comments.ToList();
            //         return post;
            //         }
            //     ).ToList();
            //     return combo;
            // }
            var allUsers = dbConnection.Query<User>(query).ToList();
            return allUsers;
        }
    }

    public void AddConnection(int cuid, int ouid)
    {
        using(IDbConnection dbConnection = Connection)
        {
            string query = $"INSERT into connections (userId1, userId2, accepted) VALUES ({cuid}, {ouid}, false)";
            dbConnection.Open();
            dbConnection.Execute(query);
        }
    }

    public void AcceptConnection(int cuid, int ouid)
    {
        using(IDbConnection dbConnection = Connection)
        {
            string query = $"UPDATE connections SET accepted = 1 WHERE userId1 = {ouid} AND userId2 = {cuid}";
            string query2 = $"INSERT into connections (userId1, userId2, accepted) VALUES ({cuid}, {ouid}, true)";
            dbConnection.Open();
            dbConnection.Execute(query);
            dbConnection.Execute(query2);
        }
    }

    public void DeleteConnection(int cuid, int ouid)
    {
        using(IDbConnection dbConnection = Connection)
        {
            string query = $"DELETE FROM connections WHERE userId1 = {ouid} AND userId2 = {cuid}";
            dbConnection.Open();
            dbConnection.Execute(query);
        }
    }


}
}