import logo from './logo.svg';
import './App.css';

function App() {
  // @ts-ignore
  const backendUrl = window._env_.BACKEND_URL;

  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.tsx</code> and save to reload.
        </p>
        {backendUrl ? `Backend URL: ${backendUrl}` : 'No backend URL configured'}
      </header>
    </div>
  );
}

export default App;
