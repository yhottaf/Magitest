using fantec.Utilities;
using UniRx;

namespace fantec.Menu.NameChange.Controller
{
    public class NameChangeController : Singleton<NameChangeController>
    {
        public Subject<string> m_DirectoryNameChange = new Subject<string>();
    }
}