using QShop.Models.ViewModel.Login;

namespace QShop.Service
{
    public class UserService
    {
        private HttpClient _client;

        public UserService(HttpClient httpClient)
        {
            _client = httpClient;
        }
        public async Task<bool> Register(AddUserViewModel model) { 
         var responce = await _client.PostAsJsonAsync("api/User/register", model);
            return responce.IsSuccessStatusCode;

        }
    }
}
