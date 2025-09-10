import { useEffect, useMemo, useState } from 'react'
import { Tabs, TabList, Tab, TabPanel } from 'react-tabs'
import 'react-tabs/style/react-tabs.css'; // Import default styles or create your own
import { api } from './api'
import type { TaskItem, User } from './types'
import TaskList from './components/TaskList'
import TaskForm from './components/TaskForm'
import UsersForm from './components/UsersForm'
import UsersList from './components/UsersList'

export default function App() {
    const [tasks, setTasks] = useState<TaskItem[]>([])
    const [users, setUsers] = useState<User[]>([])
    const [allTasks, setAllTasks] = useState<TaskItem[]>([])
    const [search, setSearch] = useState('')
    const [filter, setFilter] = useState<'all' | 'open' | 'done'>('all')
    const [filterUsers, setFilterUsers] = useState<'all' | 'active' | 'inactive'>('all')
    const [sort, setSort] = useState<'created' | 'due'>('created')
    const [loading, setLoading] = useState(true)
    const [loadingUsers, setLoadingUsers] = useState(true)

    async function load() {
        setLoading(true)
        const res = await api.get<TaskItem[]>(`/tasks`, { params: { query: search || undefined, status: filter, sort } })
        setTasks(res.data)
        setLoading(false)
    }

    async function loadAllTasks() {
        setLoading(true)
        const allTaskList = await api.get<TaskItem[]>(`/tasks`, { params: { query: search || undefined, status: 'all', sort } })
        setAllTasks(allTaskList.data)
        setLoading(false)
    }

    async function loadUsers() {
        setLoadingUsers(true)
        const res = await api.get<User[]>(`/users`)
        setUsers(res.data)
        setLoadingUsers(false)
    }



    useEffect(() => {
        load() // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [filter, sort])

    useEffect(() => {
        loadUsers() 
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [filterUsers])


    const openCount = useMemo(() => tasks.filter(t => !t.isCompleted).length, [tasks])

    const completedCount = useMemo(() => allTasks.filter(t => t.isCompleted).length, [allTasks])

    // eslint-disable-next-line react-hooks/exhaustive-deps
     useMemo(() => loadAllTasks() , [tasks])

    async function toggle(id: number) {
        const res = await api.patch<TaskItem>(`/tasks/${id}/toggle`)
        setTasks(prev => prev.map(t => t.id === id ? res.data : t))
    }

    async function deleteTask(id: number) {
        await api.delete(`/tasks/${id}`)
        setTasks(prev => prev.filter(t => t.id !== id))
    }

    async function toggleUser(id: number) {
        const res = await api.patch<User>(`/users/${id}/toggle`)
        setUsers(prev => prev.map(t => t.id === id ? res.data : t))
    }

    async function deleteUser(id: number) {
        await api.delete(`/users/${id}`)
        setUsers(prev => prev.filter(t => t.id !== id))
    }

    return (
           <Tabs>
                <TabList>
                    <Tab>Task Manager</Tab>
                    <Tab>Completed Tasks</Tab>
                    <Tab>Users</Tab>
                </TabList>
            <TabPanel>
                <div className="container">
                    <h1>Task Manager</h1>
                    <TaskForm onCreated={(t) => setTasks(prev => [t, ...prev])} />
                    <div className="space"></div>
                    <div className="card">
                        <div className="row wrap" style={{ marginBottom: '0.75rem' }}>
                            <input className="input" placeholder="Search" value={search} onChange={e => setSearch(e.target.value)} />
                            <select className="select" value={filter} onChange={e => setFilter(e.target.value as any)}>
                                <option value="all">All</option>
                                <option value="open">Open</option>
                            </select>
                            <select className="select" value={sort} onChange={e => setSort(e.target.value as any)}>
                                <option value="created">Sort: Newest</option>
                                <option value="due">Sort: Due Date</option>
                            </select>
                            <button className="btn" onClick={load}>Refresh</button>
                            <span className="badge">Open: {openCount}</span>
                        </div>
                        {loading ? <p>Loading…</p> : <TaskList tasks={tasks} filter={filter} onToggle={toggle} onDelete={deleteTask} load={load} />}
                    </div>
                </div>
            </TabPanel>
            <TabPanel>
                <div className="card">
                    <div className="row wrap" style={{ marginBottom: '0.75rem' }}>
                        <input className="input" placeholder="Search" value={search} onChange={e => setSearch(e.target.value)} />
                        <select className="select" value={sort} onChange={e => setSort(e.target.value as any)}>
                            <option value="created">Sort: Newest</option>
                            <option value="due">Sort: Due Date</option>
                        </select>
                        <button className="btn" onClick={load}>Refresh</button>
                        <span className="badge">Completed: {completedCount}</span>
                    </div>
                    {
                        loading ? <p>Loading…</p> : <TaskList tasks={allTasks} filter={'done'} onToggle={toggle} onDelete={deleteTask} load={load} />
                    }
                </div>
            </TabPanel>
            <TabPanel>
                <UsersForm onCreatedUser={(t) => setUsers(prev => [t, ...prev])} />
                <div className="space"></div>
                <div className="card">
                    <select className="select" value={filterUsers} onChange={e => setFilterUsers(e.target.value as any)}>
                        <option value="all">All</option>
                        <option value="active">Active</option>
                        <option value="inactive">InActive</option>
                    </select>
                    {loadingUsers ? <p>loading…</p> : <UsersList users={users} filter={filterUsers} onToggle={toggleUser} onDelete={deleteUser} />}
                </div>
            </TabPanel>
            </Tabs>
    )
}