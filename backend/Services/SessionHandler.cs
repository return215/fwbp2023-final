using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;

namespace backend.Services;

public class SessionHandler : IGotrueSessionPersistence<Session>
{
    private ILogger<SessionHandler> Logger { get; }
    public void DestroySession()
    {
        throw new NotImplementedException();
    }

    public Session? LoadSession()
    {
        throw new NotImplementedException();
    }

    public void SaveSession(Session session)
    {
        throw new NotImplementedException();
    }
}
