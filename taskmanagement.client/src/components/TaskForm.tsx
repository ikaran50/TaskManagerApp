import { useState } from 'react'
import { api } from '../api'
import type { TaskItem } from '../types'

interface Props {
    onCreated(task: TaskItem): void
}

export default function TaskForm({ onCreated }: Props) {
    const [title, setTitle] = useState('')
    const [description, setDescription] = useState('')
    const [dueDate, setDueDate] = useState<string>('')
    const [saving, setSaving] = useState(false)
    const [errorMessage, setErrorMessage] = useState(false)

    async function submit(e: React.FormEvent) {
        e.preventDefault()
        if (!title.trim()) return
        setSaving(true)
        try {
            const dueDateVar = new Date(dueDate);
            const payload = {
                title,
                description: description || null,
                dueDate: dueDate ? dueDateVar.toISOString() : null,
                isCompleted: false,
                UserName: ''
            }
            const now = new Date();
            if (dueDateVar < now) {
                setErrorMessage(true);
            }
            const res = await api.post<TaskItem>('/tasks', payload)
            onCreated(res.data)
            setTitle('');
            setDescription('');
            setDueDate('')
        } finally {
            setSaving(false)
        }
    }

    return (
        <>
        <form onSubmit={submit} className="card" style={{ marginTop: '1rem' }}>
            <div className="row wrap">
                <input className="input" style={{ flex: 2 }} placeholder="Task title" value={title} onChange={e => setTitle(e.target.value)} />
                <input className="input" style={{ flex: 3 }} placeholder="Description (optional)" value={description} onChange={e => setDescription(e.target.value)} />
                <input className="input" type="date" value={dueDate} onChange={e => setDueDate(e.target.value)} />
                <button className="btn primary" disabled={saving}>
                    {saving ? 'Adding…' : 'Add Task'}
                </button>
            </div>
            </form>
            {errorMessage ? (
                <div className={"error-message"}>
                        <p>
                            {"DueDate cannot be earlier then current Date"}
                        </p>
                </div>) :
             <div> </div>
             }
        </>
    )
}