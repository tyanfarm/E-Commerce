using Newtonsoft.Json;

// Session được dùng để lưu trữ thông tin về những thay đổi đối với một người dùng tại server
namespace E_Commerce.Extension {
    public static class SessionExtension {
        public static void Set<T>(this ISession session, string key, T value) {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key) {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}