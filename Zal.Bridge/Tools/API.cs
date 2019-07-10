
namespace Zal.Bridge.Tools
{
    public abstract class API
    {
        internal abstract class ENDPOINT
        {
            public const string ACTIONS = "actions";
            public const string ARTICLES = "articles";
            public const string BADGES = "badges";
            public const string DOCUMENTS = "docs";
            public const string GALLERY = "gallery";
            public const string GROUPS = "groups";
            public const string RANKS = "ranks";
            public const string USERS = "users";
            public const string SESSION = "session";
            public const string GAMES = "games";
        }

        internal abstract class METHOD
        {
            public const string GET = "Get";
            public const string GET_ALL = "GetAll";
            public const string GET_MORE = "GetMore";
            public const string ADD = "Add";
            public const string UPDATE = "Update";
            public const string DELETE = "Delete";
            public const string JOIN = "Join";
            public const string UNJOIN = "UnJoin";
            public const string JOIN_MANY = "JoinMany";
            public const string GET_CHANGED = "GetChanged";
            public const string GET_PAST_BY_YEAR = "GetAllByYear";//refactoring
            public const string REGISTER = "Register";
            public const string LOGIN = "Login";
            public const string GET_TOKEN = "GetToken";
            public const string LOGOUT = "Logout";
            public const string GET_USERS_ON_ACTION = "GetUsersOnAction";
            public const string ADD_BADGE_TO = "AddBadgeTo";
            public const string LOAD_TOP_TEN = "LoadTopTen";
            public const string LOAD_NEXT = "LoadNext";
            public const string UPLOAD_IMAGE = "UploadImage";
            public const string COMPLETE_REGISTRATION = "CompleteRegistration";
        }

        public abstract class MODE
        {
            public const string AND = "And";
            public const string OR = "Or";
        }
    }
}
