using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

public static class AuthentificationWrapper
{
    public static AuthState AuthState { get; private set; }
    private static TaskCompletionSource<AuthState> authTaskCompletionSource;

    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        Debug.Log("Starting authentication...");

        if (AuthState == AuthState.Authenticated)
        {
            Debug.Log("Already authenticated.");
            return AuthState;
        }

        if (AuthState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already in the process of authenticating.");
            return await authTaskCompletionSource.Task;
        }

        AuthState = AuthState.Authenticating;
        authTaskCompletionSource = new TaskCompletionSource<AuthState>();

        int tries = 0;

        while (AuthState == AuthState.Authenticating && tries < maxTries)
        {
            Debug.Log($"Attempt {tries + 1} to authenticate...");

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    Debug.Log("Authentication successful.");
                    authTaskCompletionSource.SetResult(AuthState);
                    break;
                }
            }
            catch (AuthenticationException ex)
            {
                Debug.LogError($"Authentication failed with exception: {ex.Message}");

                AuthState = AuthState.Error;

                authTaskCompletionSource.SetResult(AuthState);
                break;
            }

            tries++;
            Debug.LogWarning("Authentication failed. Retrying...");

            await Task.Delay(2000);
        }

        if (AuthState != AuthState.Authenticated)
        {
            Debug.LogError("Authentication failed after maximum retries.");

            AuthState = AuthState.Error;

            authTaskCompletionSource.SetResult(AuthState);
        }
        return AuthState;
    }
}

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}