import { useState, useEffect } from 'react';
import { createBridgeClient } from '@agibuild/bridge';

const bridge = createBridgeClient();

interface TodoItem {
  id: number;
  title: string;
  completed: boolean;
  createdAt: string;
}

const todoService = bridge.getService<{
  getAll: () => Promise<TodoItem[]>;
  create: (params: { title: string }) => Promise<TodoItem>;
  update: (params: { id: number; title?: string; completed?: boolean }) => Promise<TodoItem>;
  delete: (params: { id: number }) => Promise<void>;
}>('TodoService');

export function App() {
  const [ready, setReady] = useState(false);
  const [todos, setTodos] = useState<TodoItem[]>([]);
  const [input, setInput] = useState('');

  useEffect(() => {
    let cancelled = false;
    bridge.ready({ timeoutMs: 5000 }).then(() => {
      if (!cancelled) setReady(true);
    }).catch(() => {
      if (!cancelled) setReady(false);
    });
    return () => { cancelled = true; };
  }, []);

  useEffect(() => {
    if (!ready) return;
    todoService.getAll().then(setTodos).catch(() => {});
  }, [ready]);

  const addTodo = async () => {
    const title = input.trim();
    if (!title) return;
    setInput('');
    try {
      const item = await todoService.create({ title });
      setTodos((prev) => [...prev, item]);
    } catch {
      // ignore
    }
  };

  const toggleTodo = async (id: number, completed: boolean) => {
    try {
      const updated = await todoService.update({ id, completed });
      setTodos((prev) => prev.map((t) => (t.id === id ? updated : t)));
    } catch {
      // ignore
    }
  };

  const deleteTodo = async (id: number) => {
    try {
      await todoService.delete({ id });
      setTodos((prev) => prev.filter((t) => t.id !== id));
    } catch {
      // ignore
    }
  };

  if (!ready) {
    return (
      <div style={{ padding: 24, textAlign: 'center' }}>
        <p>Connecting to bridge...</p>
      </div>
    );
  }

  return (
    <div style={{ padding: 24, fontFamily: 'system-ui' }}>
      <h1>Todo</h1>
      <div style={{ display: 'flex', gap: 8, marginBottom: 16 }}>
        <input
          type="text"
          value={input}
          onChange={(e) => setInput(e.target.value)}
          onKeyDown={(e) => e.key === 'Enter' && addTodo()}
          placeholder="Add todo..."
          style={{ flex: 1, padding: 8, fontSize: 16 }}
        />
        <button onClick={addTodo} style={{ padding: '8px 16px' }}>
          Add
        </button>
      </div>
      <ul style={{ listStyle: 'none', padding: 0 }}>
        {todos.map((todo) => (
          <li
            key={todo.id}
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: 8,
              padding: 8,
              borderBottom: '1px solid #eee',
            }}
          >
            <input
              type="checkbox"
              checked={todo.completed}
              onChange={() => toggleTodo(todo.id, !todo.completed)}
            />
            <span style={{ flex: 1, textDecoration: todo.completed ? 'line-through' : 'none' }}>
              {todo.title}
            </span>
            <button onClick={() => deleteTodo(todo.id)} style={{ padding: '4px 8px' }}>
              Delete
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}
